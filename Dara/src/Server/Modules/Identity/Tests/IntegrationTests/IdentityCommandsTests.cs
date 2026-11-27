using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.GetUser;
using Dara.Server.Modules.Identity.Application.RegisterUser;
using Dara.Server.Modules.Identity.Application.SignOut;
using Dara.Server.Modules.Identity.Domain.AccountSessions;
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
        Assert.Empty(await host.ReadAsync(r => r.Query<AccountSession>().ToList()));
    }

    [Fact]
    public async Task AnAccountSessionIsValidUntilItsSignOut()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var account = await host.SignInAccountAsync("alice");
        var other = await host.SignInAccountAsync("alice");

        Assert.True(account.Succeeded);
        Assert.Equal(userId, account.UserId);
        Assert.Equal(userId, await host.AuthenticateAsync(account.AccountSessionId));

        await host.ExecuteAsync(new SignOutAccountCommand(userId, account.AccountSessionId));

        Assert.Null(await host.AuthenticateAsync(account.AccountSessionId));
        Assert.Equal(userId, await host.AuthenticateAsync(other.AccountSessionId));
        Assert.Null(await host.AuthenticateAsync(Guid.NewGuid()));
    }

    // the authentication saves the moved idle lifetime itself, outside the command pipeline
    [Fact]
    public async Task AnAuthenticatedRequestMovesTheIdleLifetimeOn()
    {
        var options = new IdentityOptions { AccountSessionIdleLifetime = TimeSpan.FromMilliseconds(600) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        var userId = await host.RegisterAsync("alice");
        var account = await host.SignInAccountAsync("alice");
        var signedInAt = await host.ReadAsync(r => r.Query<AccountSession>().Single().LastUsedAt);

        await Task.Delay(50, TestContext.Current.CancellationToken);
        Assert.Equal(userId, await host.AuthenticateAsync(account.AccountSessionId));

        Assert.True(await host.ReadAsync(r => r.Query<AccountSession>().Single().LastUsedAt) > signedInAt);
    }

    [Fact]
    public async Task AnIdleAccountSessionIsNotValid()
    {
        var options = new IdentityOptions { AccountSessionIdleLifetime = TimeSpan.FromMilliseconds(300) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        await host.RegisterAsync("alice");
        var account = await host.SignInAccountAsync("alice");

        await Task.Delay(500, TestContext.Current.CancellationToken);

        Assert.Null(await host.AuthenticateAsync(account.AccountSessionId));
    }
}
