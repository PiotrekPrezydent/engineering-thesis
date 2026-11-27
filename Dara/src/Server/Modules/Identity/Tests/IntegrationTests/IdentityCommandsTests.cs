using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;
using Dara.Server.Modules.Identity.Application.GetUser;
using Dara.Server.Modules.Identity.Application.RegisterUser;
using Dara.Server.Modules.Identity.Application.SignOut;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.UserSessions;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Rules;

namespace Dara.Server.Modules.Identity.IntegrationTests;

// accounts and the sessions of the account panel; clients are in ClientCommandsTests
public class IdentityCommandsTests
{
    [Fact]
    public async Task RegisterStoresANormalizedLoginAndOnlyAHashOfThePassword()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);

        var userId = await host.ExecuteAsync<RegisterUserCommand, Guid>(new RegisterUserCommand(" Alice.B ", "Alice@Test.local", IdentityModuleHost.Password));

        var user = await host.ReadAsync(r => r.Query<User>().Single());
        Assert.Equal(userId, user.Id.Value);
        Assert.Equal("alice.b", user.Login);
        Assert.Equal("ALICE@TEST.LOCAL", user.NormalizedEmail);
        Assert.DoesNotContain(IdentityModuleHost.Password, user.PasswordHash);

        var users = await host.QueryAsync<GetAllUsersQuery, List<(Guid userId, string login)>>(new GetAllUsersQuery());
        Assert.Equal([(userId, "alice.b")], users);
    }

    // the account and its panel client are two subjects with other ids; every sign-in of the panel acts as that client
    [Fact]
    public async Task ANewAccountGetsItsPanelClientWithItsOwnId()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");

        var panelClientId = await host.QueryAsync<GetPanelClientIdQuery, Guid?>(new GetPanelClientIdQuery(userId));
        var first = await host.SignInAccountAsync("alice");
        var second = await host.SignInAccountAsync("alice");

        Assert.NotNull(panelClientId);
        Assert.NotEqual(userId, panelClientId.Value);
        Assert.Equal(panelClientId.Value, first.PanelClientId);
        Assert.Equal(panelClientId.Value, second.PanelClientId);
        Assert.NotEqual(first.PanelSignInId, second.PanelSignInId);
        var panel = await host.ReadAsync(r => r.Query<Client>().Single());
        Assert.Equal(ClientKind.Panel, panel.Kind);
        Assert.Empty(await host.QueryAsync<GetAccountClientsQuery, List<UserClientDto>>(new GetAccountClientsQuery(userId)));
    }

    [Fact]
    public async Task RegisterRefusesATakenLoginOrEMailWhateverTheCase()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        await host.RegisterAsync("alice");

        var login = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() =>
            host.ExecuteAsync<RegisterUserCommand, Guid>(new RegisterUserCommand("ALICE", "other@test.local", IdentityModuleHost.Password)));
        Assert.IsType<LoginMustBeUniqueRule>(login.BrokenRule);

        var email = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() =>
            host.ExecuteAsync<RegisterUserCommand, Guid>(new RegisterUserCommand("bob", "ALICE@test.local", IdentityModuleHost.Password)));
        Assert.IsType<EmailMustBeUniqueRule>(email.BrokenRule);
    }

    [Fact]
    public async Task WrongPasswordAndUnknownLoginGiveTheSameError()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        await host.RegisterAsync("alice");

        var wrongPassword = await host.SignInAccountAsync("alice", "wrong-password");
        var unknownLogin = await host.SignInAccountAsync("nobody");

        Assert.Equal(AuthError.InvalidCredentials, wrongPassword.Error);
        Assert.Equal(AuthError.InvalidCredentials, unknownLogin.Error);
        Assert.Empty(await host.ReadAsync(r => r.Query<UserSession>().ToList().SelectMany(s => s.PanelSignIns).ToList()));
    }

    [Fact]
    public async Task APanelSignInIsValidUntilItsSignOut()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var account = await host.SignInAccountAsync("alice");
        var other = await host.SignInAccountAsync("alice");

        Assert.True(account.Succeeded);
        Assert.Equal(userId, account.UserId);
        Assert.Equal(userId, await host.AuthenticateAsync(account.PanelSignInId));

        await host.ExecuteAsync(new SignOutAccountCommand(userId, account.PanelSignInId));

        Assert.Null(await host.AuthenticateAsync(account.PanelSignInId));
        Assert.Equal(userId, await host.AuthenticateAsync(other.PanelSignInId));
        Assert.Null(await host.AuthenticateAsync(Guid.NewGuid()));
    }

    // the authentication saves the moved idle lifetime itself, outside the command pipeline
    [Fact]
    public async Task AnAuthenticatedRequestMovesTheIdleLifetimeOn()
    {
        var options = new IdentityOptions { PanelSignInIdleLifetime = TimeSpan.FromMilliseconds(600) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        var userId = await host.RegisterAsync("alice");
        var account = await host.SignInAccountAsync("alice");
        var signedInAt = await host.ReadAsync(r => r.Query<UserSession>().ToList().SelectMany(s => s.PanelSignIns).Single().LastUsedAt);

        await Task.Delay(50, TestContext.Current.CancellationToken);
        Assert.Equal(userId, await host.AuthenticateAsync(account.PanelSignInId));

        Assert.True(await host.ReadAsync(r => r.Query<UserSession>().ToList().SelectMany(s => s.PanelSignIns).Single().LastUsedAt) > signedInAt);
    }

    [Fact]
    public async Task AnIdlePanelSignInIsNotValid()
    {
        var options = new IdentityOptions { PanelSignInIdleLifetime = TimeSpan.FromMilliseconds(300) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        await host.RegisterAsync("alice");
        var account = await host.SignInAccountAsync("alice");

        await Task.Delay(500, TestContext.Current.CancellationToken);

        Assert.Null(await host.AuthenticateAsync(account.PanelSignInId));
    }
}
