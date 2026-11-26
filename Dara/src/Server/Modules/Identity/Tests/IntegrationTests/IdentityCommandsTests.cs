using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.GetUser;
using Dara.Server.Modules.Identity.Application.RefreshSession;
using Dara.Server.Modules.Identity.Application.RegisterUser;
using Dara.Server.Modules.Identity.Application.RevokeDeviceSession;
using Dara.Server.Modules.Identity.Application.SignOut;
using Dara.Server.Modules.Identity.Application.ValidateAccountSession;
using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Rules;
using Dara.Server.Modules.Identity.Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Identity.IntegrationTests;

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

    // a device belongs to the account that registered it first
    [Fact]
    public async Task RegisterDeviceSessionIsIdempotentAndKeepsTheFirstAccount()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var aliceId = await host.RegisterAsync("alice");
        var bobId = await host.RegisterAsync("bob");
        var deviceId = Guid.NewGuid();

        await host.RegisterDeviceAsync(aliceId, deviceId);
        await host.RegisterDeviceAsync(aliceId, deviceId);
        await host.RegisterDeviceAsync(bobId, deviceId);

        var session = await host.ReadAsync(r => r.Query<DeviceSession>().AsEnumerable().Single(s => s.Id.Value == deviceId));
        Assert.Equal(aliceId, session.UserId.Value);
        Assert.Empty(session.Tokens);
    }

    [Fact]
    public async Task ACodeSignsInTheDeviceAndOnlyHashesAreStored()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");

        var code = await host.IssueCodeAsync(userId, userId);
        Assert.True(code.Succeeded);
        Assert.Equal(8, code.Code!.Length);
        Assert.All(code.Code, c => Assert.DoesNotContain(c.ToString(), "01ILO"));
        var codeHash = await host.ReadAsync(r => r.Query<DeviceSession>().Single().LoginCode!.Hash);
        Assert.NotEqual(code.Code, codeHash);

        // typed by a person: small letters and a dash in the middle are fine
        var device = await host.SignInWithCodeAsync($"{code.Code[..4].ToLowerInvariant()}-{code.Code[4..]}");

        Assert.True(device.Succeeded);
        Assert.Equal(userId, device.UserId);
        Assert.Equal(userId, device.DeviceId);
        var session = await host.ReadAsync(r => r.Query<DeviceSession>().Single());
        Assert.Null(session.LoginCode);
        Assert.DoesNotContain(device.RefreshToken, session.Tokens.Select(t => t.TokenHash));
    }

    [Fact]
    public async Task ACodeIsUsedOnceAndANewOneReplacesTheOldOne()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");

        var first = await host.IssueCodeAsync(userId, userId);
        var second = await host.IssueCodeAsync(userId, userId);

        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync(first.Code!)).Error);
        Assert.True((await host.SignInWithCodeAsync(second.Code!)).Succeeded);
        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync(second.Code!)).Error);
        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync("")).Error);
    }

    [Fact]
    public async Task TheCodeOfAnotherAccountOrAnUnknownDeviceIsNotFound()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var aliceId = await host.RegisterAsync("alice");
        var bobId = await host.RegisterAsync("bob");

        Assert.Equal(AuthError.DeviceNotFound, (await host.IssueCodeAsync(bobId, aliceId)).Error);
        Assert.Equal(AuthError.DeviceNotFound, (await host.IssueCodeAsync(aliceId, Guid.NewGuid())).Error);
    }

    [Fact]
    public async Task AnExpiredCodeIsInvalid()
    {
        var options = new IdentityOptions { DeviceLoginCodeLifetime = TimeSpan.FromMilliseconds(300) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        var userId = await host.RegisterAsync("alice");
        var code = await host.IssueCodeAsync(userId, userId);

        await Task.Delay(500, TestContext.Current.CancellationToken);

        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync(code.Code!)).Error);
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
        Assert.Equal(userId, await ValidateAsync(host, account.AccountSessionId));

        await host.ExecuteAsync(new SignOutAccountCommand(userId, account.AccountSessionId));

        Assert.Null(await ValidateAsync(host, account.AccountSessionId));
        Assert.Equal(userId, await ValidateAsync(host, other.AccountSessionId));
        Assert.Null(await ValidateAsync(host, Guid.NewGuid()));
    }

    [Fact]
    public async Task AnIdleAccountSessionIsNotValid()
    {
        var options = new IdentityOptions { AccountSessionIdleLifetime = TimeSpan.FromMilliseconds(300) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        await host.RegisterAsync("alice");
        var account = await host.SignInAccountAsync("alice");

        await Task.Delay(500, TestContext.Current.CancellationToken);

        Assert.Null(await ValidateAsync(host, account.AccountSessionId));
    }

    [Fact]
    public async Task RefreshRotatesTheTokenAndAReusedTokenEndsTheSession()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var signIn = await host.SignInDeviceAsync(userId, userId);

        var refreshed = await RefreshAsync(host, signIn.RefreshToken!);
        Assert.True(refreshed.Succeeded);
        Assert.Equal(signIn.DeviceId, refreshed.DeviceId);
        Assert.NotEqual(signIn.RefreshToken, refreshed.RefreshToken);

        var reused = await RefreshAsync(host, signIn.RefreshToken!);
        Assert.Equal(AuthError.RefreshReused, reused.Error);

        // the token issued before the reuse was found is revoked with the whole session
        var afterReuse = await RefreshAsync(host, refreshed.RefreshToken!);
        Assert.Equal(AuthError.RefreshRevoked, afterReuse.Error);
    }

    [Fact]
    public async Task ASignedOutDeviceCannotRefreshButSignsInWithANewCode()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var device = await host.SignInDeviceAsync(userId, userId);

        await host.ExecuteAsync(new SignOutDeviceCommand(userId, device.DeviceId));

        Assert.Equal(AuthError.RefreshRevoked, (await RefreshAsync(host, device.RefreshToken!)).Error);
        Assert.True((await host.SignInDeviceAsync(userId, userId)).Succeeded);
    }

    [Fact]
    public async Task ANewSignInOfTheDeviceEndsTheTokensOfTheEarlierOne()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var first = await host.SignInDeviceAsync(userId, userId);

        var second = await host.SignInDeviceAsync(userId, userId);

        Assert.Equal(AuthError.RefreshRevoked, (await RefreshAsync(host, first.RefreshToken!)).Error);
        Assert.True((await RefreshAsync(host, second.RefreshToken!)).Succeeded);
    }

    [Fact]
    public async Task SignOutOfAnotherAccountLeavesTheSession()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var aliceId = await host.RegisterAsync("alice");
        var bobId = await host.RegisterAsync("bob");
        var device = await host.SignInDeviceAsync(aliceId, aliceId);

        await host.ExecuteAsync(new SignOutDeviceCommand(bobId, device.DeviceId));

        Assert.True((await RefreshAsync(host, device.RefreshToken!)).Succeeded);
    }

    [Fact]
    public async Task ARevokedDeviceCannotRefreshNorUseItsCodeNorGetANewOne()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var device = await host.SignInDeviceAsync(userId, userId);
        var openCode = await host.IssueCodeAsync(userId, userId);

        await host.ExecuteAsync(new RevokeDeviceSessionCommand(userId));
        await host.ExecuteAsync(new RevokeDeviceSessionCommand(userId));

        Assert.Equal(AuthError.RefreshRevoked, (await RefreshAsync(host, device.RefreshToken!)).Error);
        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync(openCode.Code!)).Error);
        Assert.Equal(AuthError.DeviceRevoked, (await host.IssueCodeAsync(userId, userId)).Error);
    }

    [Fact]
    public async Task ExpiredRefreshTokenGivesNoNewToken()
    {
        var options = new IdentityOptions { RefreshTokenLifetime = TimeSpan.FromMilliseconds(300) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        var userId = await host.RegisterAsync("alice");
        var device = await host.SignInDeviceAsync(userId, userId);

        await Task.Delay(500, TestContext.Current.CancellationToken);

        Assert.Equal(AuthError.RefreshExpired, (await RefreshAsync(host, device.RefreshToken!)).Error);
    }

    [Fact]
    public async Task UnknownRefreshTokenIsInvalid()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);

        Assert.Equal(AuthError.RefreshInvalid, (await RefreshAsync(host, "not-a-token")).Error);
        Assert.Equal(AuthError.RefreshInvalid, (await RefreshAsync(host, "")).Error);
    }

    // two refreshes that loaded the session before either was saved: the version lets only the first one in
    [Fact]
    public async Task ParallelRefreshesWithOneTokenSaveOnlyOne()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var device = await host.SignInDeviceAsync(userId, userId);

        await using var first = host.Root.CreateAsyncScope();
        await using var second = host.Root.CreateAsyncScope();
        var tokens = first.ServiceProvider.GetRequiredService<IRefreshTokenFactory>();
        var hash = tokens.Hash(device.RefreshToken!);
        var now = DateTime.UtcNow;

        var firstSession = await first.ServiceProvider.GetRequiredService<IDeviceSessionRepository>().FindByTokenHashAsync(hash);
        var secondSession = await second.ServiceProvider.GetRequiredService<IDeviceSessionRepository>().FindByTokenHashAsync(hash);
        Assert.Equal(RefreshOutcome.Refreshed, firstSession!.Refresh(hash, tokens.Create().Hash, now, TimeSpan.FromDays(1)));
        Assert.Equal(RefreshOutcome.Refreshed, secondSession!.Refresh(hash, tokens.Create().Hash, now, TimeSpan.FromDays(1)));

        await first.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync();
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync());
    }

    private static Task<DeviceTokensResult> RefreshAsync(IdentityModuleHost host, string refreshToken)
    {
        return host.ExecuteAsync<RefreshSessionCommand, DeviceTokensResult>(new RefreshSessionCommand(refreshToken));
    }

    private static Task<Guid?> ValidateAsync(IdentityModuleHost host, Guid accountSessionId)
    {
        return host.ExecuteAsync<ValidateAccountSessionCommand, Guid?>(new ValidateAccountSessionCommand(accountSessionId));
    }
}
