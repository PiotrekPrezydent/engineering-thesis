using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Auth;

namespace Dara.Server.SystemTests.ClientServer;

// the panel signs in with the password and a cookie and creates a registration code, the app of a new client
// registers itself with the code and connects to the hub with its token; an existing client signs in with a login code
public class AuthenticationTests
{
    [Fact]
    public async Task AClientRegisteredWithACodeFromThePanelConnectsAndCallsTheHub()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);

        var client = await host.ConnectAsync("auth-client", cancellationToken);
        await client.Hub.GetMyPlugins();

        Assert.NotEqual(Guid.Empty, client.UserId);
        // the server gives the client its own id, never the id of the account
        Assert.NotEqual(Guid.Empty, client.ClientId);
        Assert.NotEqual(client.UserId, client.ClientId);
    }

    [Fact]
    public async Task ThePanelSignInSetsASecureHttpOnlyStrictCookie()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var userId = await host.RegisterAsync("auth-cookie");

        var response = await host.LoginAsync(new LoginRequest("auth-cookie", ModulesHost.TestPassword));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(userId, (await response.Content.ReadFromJsonAsync<AccountSignInResponse>(cancellationToken))!.UserId);
        var cookie = Assert.Single(response.Headers.GetValues("Set-Cookie"), c => c.StartsWith(AccountCookie.CookieName + "="));
        var attributes = cookie.ToLowerInvariant();
        Assert.Contains("httponly", attributes);
        Assert.Contains("secure", attributes);
        Assert.Contains("samesite=strict", attributes);
    }

    [Fact]
    public async Task TheHubRefusesNoTokenATokenOfAnotherKeyAndThePanelCookie()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var userId = await host.RegisterAsync("auth-foreign");
        var cookie = await host.SignInAccountAsync("auth-foreign");

        var foreignIssuer = new AccessTokenIssuer(new JwtOptions { SigningKey = new string('k', 48) }, TimeProvider.System);
        var foreignToken = foreignIssuer.IssueClient(userId, Guid.NewGuid()).Token;

        Assert.Equal(HttpStatusCode.Unauthorized, (await NegotiateAsync(host, null, null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await NegotiateAsync(host, foreignToken, null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await NegotiateAsync(host, null, cookie)).StatusCode);
    }

    [Fact]
    public async Task AWrongPasswordIsInvalidCredentialsAndSetsNoCookie()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        await host.RegisterAsync("auth-wrong");

        var response = await host.LoginAsync(new LoginRequest("auth-wrong", "wrong-password"));

        await AssertProblemAsync(response, HttpStatusCode.Unauthorized, AuthErrorCodes.InvalidCredentials);
        Assert.Null(AppHubTestHost.SessionCookie(response));
    }

    [Fact]
    public async Task ARegistrationCodeRegistersOneClientAndAWrongOneIsInvalid()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var userId = await host.RegisterAsync("auth-registration");
        var cookie = await host.SignInAccountAsync("auth-registration");
        var code = await ReadCodeAsync(await host.RequestRegistrationCodeAsync(cookie));

        var registered = await host.RegisterClientWithCodeAsync(code.Code, "phone", null);
        Assert.Equal(HttpStatusCode.OK, registered.StatusCode);
        var tokens = (await registered.Content.ReadFromJsonAsync<AuthTokensResponse>(cancellationToken))!;
        Assert.Equal(userId, tokens.UserId);

        await AssertProblemAsync(await host.RegisterClientWithCodeAsync(code.Code), HttpStatusCode.Unauthorized, AuthErrorCodes.InvalidRegistrationCode);
        await AssertProblemAsync(await host.RegisterClientWithCodeAsync("ABCDEFGH"), HttpStatusCode.Unauthorized, AuthErrorCodes.InvalidRegistrationCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.RequestRegistrationCodeAsync(null)).StatusCode);
    }

    [Fact]
    public async Task AWrongOrUsedLoginCodeIsInvalid()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        await host.RegisterAsync("auth-code");
        var client = await host.RegisterClientAsync("auth-code");
        var cookie = await host.SignInAccountAsync("auth-code");
        var code = await ReadCodeAsync(await host.RequestLoginCodeAsync(cookie, client.ClientId));

        Assert.Equal(HttpStatusCode.OK, (await host.ClientLoginAsync(code.Code)).StatusCode);

        await AssertProblemAsync(await host.ClientLoginAsync(code.Code), HttpStatusCode.Unauthorized, AuthErrorCodes.InvalidLoginCode);
        await AssertProblemAsync(await host.ClientLoginAsync("ABCDEFGH"), HttpStatusCode.Unauthorized, AuthErrorCodes.InvalidLoginCode);
    }

    [Fact]
    public async Task TheLoginCodeOfAClientOfAnotherAccountIsNotFoundAndNeedsThePanelCookie()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        await host.RegisterAsync("auth-owner");
        await host.RegisterAsync("auth-other");
        var otherCookie = await host.SignInAccountAsync("auth-other");
        var client = await host.RegisterClientAsync("auth-owner");

        await AssertProblemAsync(await host.RequestLoginCodeAsync(otherCookie, client.ClientId), HttpStatusCode.NotFound, AuthErrorCodes.ClientNotFound);
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.RequestLoginCodeAsync(null, client.ClientId)).StatusCode);

        // a client token is not the panel
        using var withClientToken = new HttpRequestMessage(HttpMethod.Post, AuthRoutes.ClientLoginCode(client.ClientId));
        withClientToken.Headers.Authorization = new AuthenticationHeaderValue("Bearer", client.AccessToken);
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.Http.SendAsync(withClientToken, cancellationToken)).StatusCode);
    }

    [Fact]
    public async Task ThePanelCookieEndsWithItsSignOut()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        await host.RegisterAsync("auth-panel-out");
        var cookie = await host.SignInAccountAsync("auth-panel-out");

        var logout = await host.SendAsPanelAsync(HttpMethod.Post, AuthRoutes.Logout, cookie);

        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        // the answer removes the cookie in the browser, and the server refuses the old one anyway
        Assert.Contains(logout.Headers.GetValues("Set-Cookie"), c => c.StartsWith(AccountCookie.CookieName + "=;"));
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.RequestRegistrationCodeAsync(cookie)).StatusCode);
    }

    [Fact]
    public async Task LogoutWithoutATokenOrCookieIsUnauthorized()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);

        var response = await host.Http.PostAsync(AuthRoutes.Logout, null, cancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshRotatesTheTokenAndAReusedOneEndsTheSession()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        await host.RegisterAsync("auth-rotation");
        var signIn = await host.RegisterClientAsync("auth-rotation");

        var refreshResponse = await RefreshAsync(host, signIn.RefreshToken);
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var refreshed = (await refreshResponse.Content.ReadFromJsonAsync<AuthTokensResponse>(cancellationToken))!;
        Assert.Equal(signIn.ClientId, refreshed.ClientId);
        Assert.NotEqual(signIn.RefreshToken, refreshed.RefreshToken);

        await AssertProblemAsync(await RefreshAsync(host, signIn.RefreshToken), HttpStatusCode.Unauthorized, AuthErrorCodes.RefreshReused);
        await AssertProblemAsync(await RefreshAsync(host, refreshed.RefreshToken), HttpStatusCode.Unauthorized, AuthErrorCodes.RefreshRevoked);
    }

    [Fact]
    public async Task ASignedOutClientCannotBeRefreshedButSignsInWithALoginCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        await host.RegisterAsync("auth-logout");
        var signIn = await host.RegisterClientAsync("auth-logout");

        using var logout = new HttpRequestMessage(HttpMethod.Post, AuthRoutes.Logout);
        logout.Headers.Authorization = new AuthenticationHeaderValue("Bearer", signIn.AccessToken);
        Assert.Equal(HttpStatusCode.NoContent, (await host.Http.SendAsync(logout, cancellationToken)).StatusCode);

        await AssertProblemAsync(await RefreshAsync(host, signIn.RefreshToken), HttpStatusCode.Unauthorized, AuthErrorCodes.RefreshRevoked);
        Assert.Equal(signIn.ClientId, (await host.SignInClientAsync("auth-logout", signIn.ClientId)).ClientId);
    }

    // a code has about 40 bits, so guessing is slowed down per client address, for both kinds of code together
    [Fact]
    public async Task TheCodeEndpointsRefuseTooManyAttempts()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, clientCodePermitsPerMinute: 3);

        for (var i = 0; i < 2; i++)
            Assert.Equal(HttpStatusCode.Unauthorized, (await host.ClientLoginAsync("ABCDEFGH")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.RegisterClientWithCodeAsync("ABCDEFGH")).StatusCode);

        Assert.Equal(HttpStatusCode.TooManyRequests, (await host.ClientLoginAsync("ABCDEFGH")).StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, (await host.RegisterClientWithCodeAsync("ABCDEFGH")).StatusCode);
    }

    // the server closes the connection when the client token expires; the client refreshes and connects again
    [Fact]
    public async Task AnExpiredTokenClosesTheConnectionAndARefreshedOneConnectsAgain()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, accessTokenLifetime: TimeSpan.FromSeconds(2));
        var client = await host.ConnectAsync("auth-expiring", cancellationToken);
        var firstToken = client.Tokens.AccessToken;

        var closed = new TaskCompletionSource<Exception?>(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Connection.Closed += exception =>
        {
            closed.TrySetResult(exception);
            return Task.CompletedTask;
        };

        await closed.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);

        await client.ReconnectAsync(cancellationToken);
        await client.Hub.GetMyPlugins();
        Assert.NotEqual(firstToken, client.Tokens.AccessToken);
    }

    private static async Task<ClientCodeResponse> ReadCodeAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ClientCodeResponse>())!;
    }

    private static async Task<HttpResponseMessage> NegotiateAsync(AppHubTestHost host, string? accessToken, string? cookie)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/{Connections.HubName}/negotiate?negotiateVersion=1");
        if (accessToken != null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        if (cookie != null)
            request.Headers.Add("Cookie", cookie);

        return await host.Http.SendAsync(request);
    }

    private static Task<HttpResponseMessage> RefreshAsync(AppHubTestHost host, string refreshToken)
    {
        return host.Http.PostAsJsonAsync(AuthRoutes.Refresh, new RefreshRequest(refreshToken));
    }

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        Assert.Equal(status, response.StatusCode);
        using var problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(code, problem.RootElement.GetProperty("code").GetString());
    }
}
