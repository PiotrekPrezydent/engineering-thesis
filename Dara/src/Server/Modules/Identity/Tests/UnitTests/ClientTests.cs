using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Clients.Events;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.UnitTests;

// the clients of an account: a client of the hub registers with a one-time code of the panel, the panel client comes
// with the account; each is its own aggregate
public class ClientTests
{
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
    private static readonly UserId UserId = new(Guid.NewGuid());

    private static User NewUser(string login = "alice")
    {
        var user = User.Register(login, login + "@test.local", "password-1234", Now, new FakePasswordHasher(), new FakeUniquenessChecker());
        user.ClearDomainEvents();
        return user;
    }

    private static Client Register(ClientType type = ClientType.Pc, string? name = null, int sameTypeCount = 0, UserId? userId = null)
    {
        var client = Client.RegisterHub(userId ?? UserId, type, name, sameTypeCount, Now);
        client.ClearDomainEvents();
        return client;
    }

    private static void AssertBroken<TRule>(Action action) where TRule : IBuisnessRule
    {
        var refused = Assert.Throws<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(refused.BrokenRule);
    }

    [Fact]
    public void ARegistrationCodeIsUsedOnce()
    {
        var user = NewUser();
        user.CreateClientRegistrationCode("r1", Now, CodeLifetime);

        Assert.True(user.UseClientRegistrationCode("r1", Now.AddMinutes(1)));
        Assert.Empty(user.ClientRegistrationCodes);
        Assert.False(user.UseClientRegistrationCode("r1", Now.AddMinutes(2)));
    }

    [Fact]
    public void AnUnknownOrExpiredRegistrationCodeIsRefused()
    {
        var user = NewUser();
        user.CreateClientRegistrationCode("r1", Now, CodeLifetime);

        Assert.False(user.UseClientRegistrationCode("other", Now));
        Assert.False(user.UseClientRegistrationCode("r1", Now + CodeLifetime));
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
    public void ARegisteredClientOfTheHubRaisesItsEvent()
    {
        var client = Client.RegisterHub(UserId, ClientType.Phone, " My phone ", 0, Now);

        Assert.Equal(UserId, client.UserId);
        Assert.Equal(ClientKind.Hub, client.Kind);
        Assert.Equal("My phone", client.Hub!.Name);
        Assert.Equal(ClientType.Phone, client.Hub.Type);
        Assert.Empty(client.Hub.Context.Entries);
        var registered = Assert.IsType<UserClientRegisteredDomainEvent>(Assert.Single(client.DomainEvents));
        Assert.Equal(client.Id, registered.ClientId);
    }

    // the count of clients of the type in the account (revoked ones too) comes from the application
    [Fact]
    public void AnEmptyNameGetsTheLabelOfTheTypeWithTheNextNumber()
    {
        Assert.Equal("PC 1", Register(ClientType.Pc).Hub!.Name);
        Assert.Equal("PC 2", Register(ClientType.Pc, " ", sameTypeCount: 1).Hub!.Name);
        Assert.Equal("Phone 1", Register(ClientType.Phone).Hub!.Name);
    }

    [Fact]
    public void AnInvalidNameIsRefused()
    {
        AssertBroken<ClientNameMustBeValidRule>(() => Client.RegisterHub(UserId, ClientType.Pc, new string('n', 51), 0, Now));
    }

    [Fact]
    public void ALoginCodeSignsInItsClientOnce()
    {
        var client = Register();
        client.IssueLoginCode("l1", Now, CodeLifetime);

        Assert.True(client.HasLoginCode("l1"));
        Assert.True(client.ConsumeLoginCode("l1", Now.AddMinutes(1)));
        Assert.Null(client.Hub!.LoginCode);
        Assert.False(client.ConsumeLoginCode("l1", Now.AddMinutes(2)));
    }

    [Fact]
    public void ANewLoginCodeReplacesTheOldOneAndACodeExpires()
    {
        var client = Register();
        client.IssueLoginCode("l1", Now, CodeLifetime);
        client.IssueLoginCode("l2", Now, CodeLifetime);

        Assert.False(client.ConsumeLoginCode("l1", Now));
        Assert.False(client.ConsumeLoginCode("l2", Now + CodeLifetime));
        Assert.NotNull(client.Hub!.LoginCode);
    }

    [Fact]
    public void RenameAndContextChangeRaiseEventsOnlyOnAChange()
    {
        var client = Register();

        client.Rename(" Laptop ");
        client.Rename("Laptop");
        client.ChangeContext(new Dictionary<string, string> { [" Photos "] = " D:\\Photos " });
        client.ChangeContext(new Dictionary<string, string> { ["Photos"] = "D:\\Photos" });

        Assert.Equal("Laptop", client.Hub!.Name);
        Assert.Equal([new ClientContextEntry("Photos", "D:\\Photos")], client.Hub.Context.Entries);
        Assert.Collection(client.DomainEvents,
            e => Assert.IsType<UserClientRenamedDomainEvent>(e),
            e => Assert.IsType<UserClientContextChangedDomainEvent>(e));
    }

    [Fact]
    public void AnInvalidContextIsRefused()
    {
        var client = Register();

        AssertBroken<ClientContextMustBeValidRule>(() => client.ChangeContext(new Dictionary<string, string> { [" "] = "x" }));
    }

    [Fact]
    public void AClientOfAnotherAccountIsRefused()
    {
        var bobsClient = Register(userId: new UserId(Guid.NewGuid()));

        AssertBroken<ClientMustBelongToUserRule>(() => bobsClient.EnsureOwnedBy(UserId));
        bobsClient.EnsureOwnedBy(bobsClient.UserId);
    }

    [Fact]
    public void ARevokedClientLosesItsCodeAndTakesNoChange()
    {
        var client = Register();
        client.IssueLoginCode("l1", Now, CodeLifetime);

        client.Revoke(Now);

        Assert.True(client.IsRevoked);
        Assert.Null(client.Hub!.LoginCode);
        Assert.IsType<UserClientRevokedDomainEvent>(Assert.Single(client.DomainEvents));
        Assert.False(client.HasLoginCode("l1"));
        AssertBroken<ClientMustNotBeRevokedRule>(() => client.Revoke(Now));
        AssertBroken<ClientMustNotBeRevokedRule>(() => client.Rename("Other"));
        AssertBroken<ClientMustNotBeRevokedRule>(() => client.IssueLoginCode("l2", Now, CodeLifetime));
        AssertBroken<ClientMustNotBeRevokedRule>(() => client.ChangeContext(new Dictionary<string, string>()));
    }

    // the panel client comes with the account and has nothing of a client of the hub
    [Fact]
    public void ThePanelClientTakesNoChangeOfAHubClient()
    {
        var panel = Client.CreatePanel(UserId, Now);

        Assert.Equal(ClientKind.Panel, panel.Kind);
        Assert.Null(panel.Hub);
        var created = Assert.IsType<PanelClientCreatedDomainEvent>(Assert.Single(panel.DomainEvents));
        Assert.Equal(panel.Id, created.ClientId);
        Assert.Equal(UserId, created.UserId);
        Assert.False(panel.HasLoginCode("l1"));
        AssertBroken<ClientMustBeHubClientRule>(() => panel.Revoke(Now));
        AssertBroken<ClientMustBeHubClientRule>(() => panel.Rename("Panel"));
        AssertBroken<ClientMustBeHubClientRule>(() => panel.IssueLoginCode("l1", Now, CodeLifetime));
        AssertBroken<ClientMustBeHubClientRule>(() => panel.ChangeContext(new Dictionary<string, string>()));
    }
}
