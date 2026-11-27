using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.ChangeClientContext;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;
using Dara.Server.Modules.Identity.Application.RefreshSession;
using Dara.Server.Modules.Identity.Application.RenameClient;
using Dara.Server.Modules.Identity.Application.RevokeClient;
using Dara.Server.Modules.Identity.Application.SignOut;
using Dara.Server.Modules.Identity.Domain.UserSessions;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Identity.IntegrationTests;

// clients of an account: registration with a code of the panel, login codes, refresh tokens, changes and revocation
public class ClientCommandsTests
{
    [Fact]
    public async Task ARegistrationCodeRegistersOneClientWithItsSessionAndOnlyHashesAreStored()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");

        var code = await host.CreateRegistrationCodeAsync(userId);
        Assert.True(code.Succeeded);
        Assert.Equal(8, code.Code!.Length);
        Assert.All(code.Code, c => Assert.DoesNotContain(c.ToString(), "01ILO"));
        var codeHash = await host.ReadAsync(r => r.Query<User>().Single().ClientRegistrationCodes.Single().Hash);
        Assert.NotEqual(code.Code, codeHash);

        // typed by a person: small letters and a dash in the middle are fine
        var client = await host.RegisterClientWithCodeAsync($"{code.Code[..4].ToLowerInvariant()}-{code.Code[4..]}", "phone", null);

        Assert.True(client.Succeeded);
        Assert.Equal(userId, client.UserId);
        Assert.NotEqual(Guid.Empty, client.ClientId);
        var user = await host.ReadAsync(r => r.Query<User>().Single());
        Assert.Empty(user.ClientRegistrationCodes);
        var registered = await host.ReadAsync(r => r.Query<Client>().Single(c => c.Kind == ClientKind.Hub));
        Assert.Equal(client.ClientId, registered.Id.Value);
        Assert.Equal(ClientType.Phone, registered.Hub!.Type);
        Assert.Equal("Phone 1", registered.Hub.Name);

        var session = await host.ReadAsync(r => r.Query<UserSession>().Single(s => s.Kind == ClientKind.Hub));
        Assert.Equal(client.ClientId, session.ClientId.Value);
        Assert.DoesNotContain(client.RefreshToken, session.Tokens.Select(t => t.TokenHash));
    }

    [Fact]
    public async Task ARegistrationCodeIsUsedOnceAndAnUnknownOneIsInvalid()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var code = await host.CreateRegistrationCodeAsync(userId);

        Assert.True((await host.RegisterClientWithCodeAsync(code.Code!)).Succeeded);
        Assert.Equal(AuthError.InvalidRegistrationCode, (await host.RegisterClientWithCodeAsync(code.Code!)).Error);
        Assert.Equal(AuthError.InvalidRegistrationCode, (await host.RegisterClientWithCodeAsync("")).Error);
        Assert.Equal(AuthError.InvalidRegistrationCode, (await host.RegisterClientWithCodeAsync("ABCDEFGH")).Error);
    }

    [Fact]
    public async Task AnExpiredRegistrationCodeIsInvalid()
    {
        var options = new IdentityOptions { ClientRegistrationCodeLifetime = TimeSpan.FromMilliseconds(300) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        var userId = await host.RegisterAsync("alice");
        var code = await host.CreateRegistrationCodeAsync(userId);

        await Task.Delay(500, TestContext.Current.CancellationToken);

        Assert.Equal(AuthError.InvalidRegistrationCode, (await host.RegisterClientWithCodeAsync(code.Code!)).Error);
    }

    [Fact]
    public async Task ALoginCodeSignsInAnExistingClientAndEndsTheTokensOfTheEarlierCopy()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var registered = await host.RegisterClientAsync(userId);

        var code = await host.IssueLoginCodeAsync(userId, registered.ClientId);
        Assert.True(code.Succeeded);
        var signedIn = await host.SignInWithCodeAsync(code.Code!.ToLowerInvariant());

        Assert.True(signedIn.Succeeded);
        Assert.Equal(registered.ClientId, signedIn.ClientId);
        Assert.Equal(AuthError.RefreshRevoked, (await RefreshAsync(host, registered.RefreshToken!)).Error);
        Assert.True((await RefreshAsync(host, signedIn.RefreshToken!)).Succeeded);
        Assert.Null(await host.ReadAsync(r => r.Query<Client>().Single(c => c.Kind == ClientKind.Hub).Hub!.LoginCode));
    }

    [Fact]
    public async Task ALoginCodeIsUsedOnceAndANewOneReplacesTheOldOne()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var client = await host.RegisterClientAsync(userId);

        var first = await host.IssueLoginCodeAsync(userId, client.ClientId);
        var second = await host.IssueLoginCodeAsync(userId, client.ClientId);

        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync(first.Code!)).Error);
        Assert.True((await host.SignInWithCodeAsync(second.Code!)).Succeeded);
        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync(second.Code!)).Error);
        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync("")).Error);
    }

    [Fact]
    public async Task TheLoginCodeOfAClientOfAnotherAccountOrAnUnknownClientIsNotFound()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var aliceId = await host.RegisterAsync("alice");
        var bobId = await host.RegisterAsync("bob");
        var alicesClient = await host.RegisterClientAsync(aliceId);

        Assert.Equal(AuthError.ClientNotFound, (await host.IssueLoginCodeAsync(bobId, alicesClient.ClientId)).Error);
        Assert.Equal(AuthError.ClientNotFound, (await host.IssueLoginCodeAsync(aliceId, Guid.NewGuid())).Error);
    }

    [Fact]
    public async Task AnExpiredLoginCodeIsInvalid()
    {
        var options = new IdentityOptions { ClientLoginCodeLifetime = TimeSpan.FromMilliseconds(300) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        var userId = await host.RegisterAsync("alice");
        var client = await host.RegisterClientAsync(userId);
        var code = await host.IssueLoginCodeAsync(userId, client.ClientId);

        await Task.Delay(500, TestContext.Current.CancellationToken);

        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync(code.Code!)).Error);
    }

    [Fact]
    public async Task RefreshRotatesTheTokenAndAReusedTokenEndsTheSession()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var client = await host.RegisterClientAsync(userId);

        var refreshed = await RefreshAsync(host, client.RefreshToken!);
        Assert.True(refreshed.Succeeded);
        Assert.Equal(client.ClientId, refreshed.ClientId);
        Assert.Equal(userId, refreshed.UserId);
        Assert.NotEqual(client.RefreshToken, refreshed.RefreshToken);

        var reused = await RefreshAsync(host, client.RefreshToken!);
        Assert.Equal(AuthError.RefreshReused, reused.Error);

        // the token issued before the reuse was found is revoked with the whole session
        var afterReuse = await RefreshAsync(host, refreshed.RefreshToken!);
        Assert.Equal(AuthError.RefreshRevoked, afterReuse.Error);
    }

    [Fact]
    public async Task ASignedOutClientCannotRefreshButSignsInWithALoginCode()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var client = await host.RegisterClientAsync(userId);

        await host.ExecuteAsync(new SignOutClientCommand(userId, client.ClientId));

        Assert.Equal(AuthError.RefreshRevoked, (await RefreshAsync(host, client.RefreshToken!)).Error);
        Assert.True((await host.SignInClientAsync(userId, client.ClientId)).Succeeded);
    }

    [Fact]
    public async Task SignOutOfAnotherAccountLeavesTheSession()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var aliceId = await host.RegisterAsync("alice");
        var bobId = await host.RegisterAsync("bob");
        var client = await host.RegisterClientAsync(aliceId);

        await host.ExecuteAsync(new SignOutClientCommand(bobId, client.ClientId));

        Assert.True((await RefreshAsync(host, client.RefreshToken!)).Succeeded);
    }

    [Fact]
    public async Task ARevokedClientCannotRefreshNorUseItsCodeNorGetANewOneNorChange()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var userId = await host.RegisterAsync("alice");
        var client = await host.RegisterClientAsync(userId);
        var openCode = await host.IssueLoginCodeAsync(userId, client.ClientId);

        await host.ExecuteAsync(new RevokeClientCommand(userId, client.ClientId));

        Assert.Equal(AuthError.RefreshRevoked, (await RefreshAsync(host, client.RefreshToken!)).Error);
        Assert.Equal(AuthError.InvalidLoginCode, (await host.SignInWithCodeAsync(openCode.Code!)).Error);
        Assert.Equal(AuthError.ClientRevoked, (await host.IssueLoginCodeAsync(userId, client.ClientId)).Error);
        var rename = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() =>
            host.ExecuteAsync(new RenameClientCommand(userId, client.ClientId, "Other")));
        Assert.IsType<ClientMustNotBeRevokedRule>(rename.BrokenRule);
        Assert.NotNull(await host.ReadAsync(r => r.Query<UserSession>().Single(s => s.Kind == ClientKind.Hub).RevokedAt));
    }

    [Fact]
    public async Task AChangeOfAClientOfAnotherAccountBreaksTheRule()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var aliceId = await host.RegisterAsync("alice");
        var bobId = await host.RegisterAsync("bob");
        var client = await host.RegisterClientAsync(aliceId);

        var rename = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() =>
            host.ExecuteAsync(new RenameClientCommand(bobId, client.ClientId, "Bob's now")));
        Assert.IsType<ClientMustBelongToUserRule>(rename.BrokenRule);
        var revoke = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() =>
            host.ExecuteAsync(new RevokeClientCommand(bobId, client.ClientId)));
        Assert.IsType<ClientMustBelongToUserRule>(revoke.BrokenRule);
        Assert.True((await RefreshAsync(host, client.RefreshToken!)).Succeeded);
    }

    [Fact]
    public async Task RenameAndContextAreSavedAndTheQueriesShowThem()
    {
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var aliceId = await host.RegisterAsync("alice");
        var bobId = await host.RegisterAsync("bob");
        var first = await host.RegisterClientAsync(aliceId, "pc", "Desk");
        var second = await host.RegisterClientAsync(aliceId, "phone");

        await host.ExecuteAsync(new RenameClientCommand(aliceId, first.ClientId, "  Work PC "));
        await host.ExecuteAsync(new ChangeClientContextCommand(aliceId, first.ClientId, new Dictionary<string, string> { [" Photos "] = " D:\\Photos " }));
        await host.ExecuteAsync(new ChangeClientContextCommand(aliceId, first.ClientId, new Dictionary<string, string> { ["Photos"] = "E:\\Photos", ["Music"] = "E:\\Music" }));

        var all = await host.QueryAsync<GetAccountClientsQuery, List<UserClientDto>>(new GetAccountClientsQuery(aliceId));
        Assert.Equal([first.ClientId, second.ClientId], all.Select(c => c.ClientId));
        var changed = all[0];
        Assert.Equal("Work PC", changed.Name);
        Assert.Equal("Pc", changed.ClientType);
        Assert.Equal(new Dictionary<string, string> { ["Photos"] = "E:\\Photos", ["Music"] = "E:\\Music" }, changed.Context);

        Assert.NotNull(await host.QueryAsync<GetAccountClientQuery, UserClientDto?>(new GetAccountClientQuery(aliceId, first.ClientId)));
        Assert.Null(await host.QueryAsync<GetAccountClientQuery, UserClientDto?>(new GetAccountClientQuery(bobId, first.ClientId)));
        Assert.Equal("Work PC", (await host.QueryAsync<GetCurrentClientQuery, UserClientDto?>(new GetCurrentClientQuery(first.ClientId)))!.Name);
        Assert.Equal("Phone 1", (await host.QueryAsync<GetClientQuery, ClientDto?>(new GetClientQuery(second.ClientId)))!.Name);
        Assert.Null(await host.QueryAsync<GetClientQuery, ClientDto?>(new GetClientQuery(Guid.NewGuid())));
        var some = await host.QueryAsync<GetClientsQuery, List<ClientDto>>(new GetClientsQuery([second.ClientId, Guid.NewGuid()]));
        Assert.Equal(second.ClientId, Assert.Single(some).ClientId);
        Assert.Empty(await host.QueryAsync<GetAccountClientsQuery, List<UserClientDto>>(new GetAccountClientsQuery(bobId)));
    }

    [Fact]
    public async Task ExpiredRefreshTokenGivesNoNewToken()
    {
        var options = new IdentityOptions { RefreshTokenLifetime = TimeSpan.FromMilliseconds(300) };
        await using var host = await IdentityModuleHost.StartAsync(TestContext.Current.CancellationToken, options);
        var userId = await host.RegisterAsync("alice");
        var client = await host.RegisterClientAsync(userId);

        await Task.Delay(500, TestContext.Current.CancellationToken);

        Assert.Equal(AuthError.RefreshExpired, (await RefreshAsync(host, client.RefreshToken!)).Error);
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
        var client = await host.RegisterClientAsync(userId);

        await using var first = host.Root.CreateAsyncScope();
        await using var second = host.Root.CreateAsyncScope();
        var tokens = first.ServiceProvider.GetRequiredService<IRefreshTokenFactory>();
        var hash = tokens.Hash(client.RefreshToken!);
        var now = DateTime.UtcNow;

        var firstSession = await first.ServiceProvider.GetRequiredService<IUserSessionRepository>().FindByTokenHashAsync(hash);
        var secondSession = await second.ServiceProvider.GetRequiredService<IUserSessionRepository>().FindByTokenHashAsync(hash);
        Assert.Equal(RefreshOutcome.Refreshed, firstSession!.Refresh(hash, tokens.Create().Hash, now, TimeSpan.FromDays(1)));
        Assert.Equal(RefreshOutcome.Refreshed, secondSession!.Refresh(hash, tokens.Create().Hash, now, TimeSpan.FromDays(1)));

        await first.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync();
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync());
    }

    private static Task<ClientTokensResult> RefreshAsync(IdentityModuleHost host, string refreshToken)
    {
        return host.ExecuteAsync<RefreshSessionCommand, ClientTokensResult>(new RefreshSessionCommand(refreshToken));
    }
}
