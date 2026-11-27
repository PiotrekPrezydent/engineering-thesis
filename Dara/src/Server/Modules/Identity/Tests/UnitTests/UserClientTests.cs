using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Clients;
using Dara.Server.Modules.Identity.Domain.Users.Events;
using Dara.Server.Modules.Identity.Domain.Users.Rules;

namespace Dara.Server.Modules.Identity.UnitTests;

// the clients of an account: registered with a one-time code of the panel and changed only through User
public class UserClientTests
{
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);

    private static User NewUser(string login = "alice")
    {
        var user = User.Register(login, login + "@test.local", "password-1234", Now, new FakePasswordHasher(), new FakeUniquenessChecker());
        user.ClearDomainEvents();
        return user;
    }

    private static UserClient Register(User user, ClientType type = ClientType.Pc, string? name = null)
    {
        var codeHash = "r-" + Guid.NewGuid();
        user.CreateClientRegistrationCode(codeHash, Now, CodeLifetime);
        return user.RegisterClient(codeHash, type, name, Now)!;
    }

    private static void AssertBroken<TRule>(Action action) where TRule : IBuisnessRule
    {
        var refused = Assert.Throws<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(refused.BrokenRule);
    }

    [Fact]
    public void ARegistrationCodeAddsAClientOnce()
    {
        var user = NewUser();
        user.CreateClientRegistrationCode("r1", Now, CodeLifetime);

        var client = user.RegisterClient("r1", ClientType.Phone, " My phone ", Now.AddMinutes(1));

        Assert.NotNull(client);
        Assert.Equal(user.Id, client.UserId);
        Assert.Equal("My phone", client.Name);
        Assert.Equal(ClientType.Phone, client.Type);
        Assert.Empty(client.Context.Entries);
        Assert.Empty(user.ClientRegistrationCodes);
        var registered = Assert.IsType<UserClientRegisteredDomainEvent>(Assert.Single(user.DomainEvents));
        Assert.Equal(client.Id, registered.ClientId);
        Assert.Null(user.RegisterClient("r1", ClientType.Phone, null, Now.AddMinutes(2)));
        Assert.Single(user.Clients);
    }

    [Fact]
    public void AnUnknownOrExpiredRegistrationCodeAddsNoClient()
    {
        var user = NewUser();
        user.CreateClientRegistrationCode("r1", Now, CodeLifetime);

        Assert.Null(user.RegisterClient("other", ClientType.Pc, null, Now));
        Assert.Null(user.RegisterClient("r1", ClientType.Pc, null, Now + CodeLifetime));
        Assert.Empty(user.Clients);
    }

    [Fact]
    public void ANewRegistrationCodeRemovesExpiredOnesAndKeepsOpenOnes()
    {
        var user = NewUser();
        user.CreateClientRegistrationCode("r1", Now, CodeLifetime);
        user.CreateClientRegistrationCode("r2", Now.AddMinutes(5), CodeLifetime);

        user.CreateClientRegistrationCode("r3", Now + CodeLifetime, CodeLifetime);

        Assert.Equal(["r2", "r3"], user.ClientRegistrationCodes.Select(c => c.Hash));
    }

    [Fact]
    public void AnEmptyNameGetsTheLabelOfTheTypeCountingRevokedClients()
    {
        var user = NewUser();
        var first = Register(user);
        user.RevokeClient(first.Id, Now);

        var second = Register(user, ClientType.Pc, " ");
        var phone = Register(user, ClientType.Phone);

        Assert.Equal("PC 1", first.Name);
        Assert.Equal("PC 2", second.Name);
        Assert.Equal("Phone 1", phone.Name);
    }

    [Fact]
    public void AnInvalidNameIsRefusedAndTheCodeStays()
    {
        var user = NewUser();
        user.CreateClientRegistrationCode("r1", Now, CodeLifetime);

        AssertBroken<ClientNameMustBeValidRule>(() => user.RegisterClient("r1", ClientType.Pc, new string('n', 51), Now));
        Assert.Single(user.ClientRegistrationCodes);
    }

    [Fact]
    public void ALoginCodeSignsInItsClientOnce()
    {
        var user = NewUser();
        var client = Register(user);
        user.IssueClientLoginCode(client.Id, "l1", Now, CodeLifetime);

        Assert.Equal(client.Id, user.ConsumeClientLoginCode("l1", Now.AddMinutes(1)));
        Assert.Null(client.LoginCode);
        Assert.Null(user.ConsumeClientLoginCode("l1", Now.AddMinutes(2)));
    }

    [Fact]
    public void ANewLoginCodeReplacesTheOldOneAndACodeExpires()
    {
        var user = NewUser();
        var client = Register(user);
        user.IssueClientLoginCode(client.Id, "l1", Now, CodeLifetime);
        user.IssueClientLoginCode(client.Id, "l2", Now, CodeLifetime);

        Assert.Null(user.ConsumeClientLoginCode("l1", Now));
        Assert.Null(user.ConsumeClientLoginCode("l2", Now + CodeLifetime));
        Assert.NotNull(client.LoginCode);
    }

    [Fact]
    public void RenameAndContextChangeRaiseEventsOnlyOnAChange()
    {
        var user = NewUser();
        var client = Register(user);
        user.ClearDomainEvents();

        user.RenameClient(client.Id, " Laptop ");
        user.RenameClient(client.Id, "Laptop");
        user.ChangeClientContext(client.Id, new Dictionary<string, string> { [" Photos "] = " D:\\Photos " });
        user.ChangeClientContext(client.Id, new Dictionary<string, string> { ["Photos"] = "D:\\Photos" });

        Assert.Equal("Laptop", client.Name);
        Assert.Equal([new ClientContextEntry("Photos", "D:\\Photos")], client.Context.Entries);
        Assert.Collection(user.DomainEvents,
            e => Assert.IsType<UserClientRenamedDomainEvent>(e),
            e => Assert.IsType<UserClientContextChangedDomainEvent>(e));
    }

    [Fact]
    public void AnInvalidContextIsRefused()
    {
        var user = NewUser();
        var client = Register(user);

        AssertBroken<ClientContextMustBeValidRule>(() => user.ChangeClientContext(client.Id, new Dictionary<string, string> { [" "] = "x" }));
    }

    [Fact]
    public void AClientOfAnotherAccountIsRefused()
    {
        var alice = NewUser("alice");
        var bob = NewUser("bob");
        var bobsClient = Register(bob);

        AssertBroken<ClientMustBelongToUserRule>(() => alice.RenameClient(bobsClient.Id, "Mine"));
        AssertBroken<ClientMustBelongToUserRule>(() => alice.RevokeClient(bobsClient.Id, Now));
        AssertBroken<ClientMustBelongToUserRule>(() => alice.IssueClientLoginCode(bobsClient.Id, "l1", Now, CodeLifetime));
        AssertBroken<ClientMustBelongToUserRule>(() => alice.ChangeClientContext(bobsClient.Id, new Dictionary<string, string>()));
        Assert.Equal("PC 1", bobsClient.Name);
    }

    [Fact]
    public void ARevokedClientLosesItsCodeAndTakesNoChange()
    {
        var user = NewUser();
        var client = Register(user);
        user.IssueClientLoginCode(client.Id, "l1", Now, CodeLifetime);
        user.ClearDomainEvents();

        user.RevokeClient(client.Id, Now);

        Assert.True(client.IsRevoked);
        Assert.Null(client.LoginCode);
        Assert.IsType<UserClientRevokedDomainEvent>(Assert.Single(user.DomainEvents));
        Assert.Null(user.ConsumeClientLoginCode("l1", Now));
        AssertBroken<ClientMustNotBeRevokedRule>(() => user.RevokeClient(client.Id, Now));
        AssertBroken<ClientMustNotBeRevokedRule>(() => user.RenameClient(client.Id, "Other"));
        AssertBroken<ClientMustNotBeRevokedRule>(() => user.IssueClientLoginCode(client.Id, "l2", Now, CodeLifetime));
        AssertBroken<ClientMustNotBeRevokedRule>(() => user.ChangeClientContext(client.Id, new Dictionary<string, string>()));
    }
}
