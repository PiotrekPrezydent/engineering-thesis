using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Dara.Server.Apps.API.Authentication;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Auth;

namespace Dara.Server.SystemTests.ClientServer;

// the panel signs in with the password and a cookie, issues a code for its device, the device signs in with the code
// and connects to the hub with its token
public class AuthenticationTests
{
    [Fact]
    public async Task ADeviceSignedInWithACodeFromThePanelConnectsAndCallsTheHub()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);

        var device = await host.ConnectAsync("auth-device", cancellationToken);
        await device.Hub.ChangeName("Auth Device");

        Assert.NotEqual(Guid.Empty, device.UserId);
        // the seed and the tests give every account one device with the id of the account
        Assert.Equal(device.UserId, device.DeviceId);
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
        var foreignToken = foreignIssuer.IssueDevice(userId, userId).Token;

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
    public async Task AWrongOrUsedCodeIsInvalid()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var userId = await host.RegisterAsync("auth-code");
        var cookie = await host.SignInAccountAsync("auth-code");
        var code = await IssueCodeAsync(host, cookie, userId);

        Assert.Equal(HttpStatusCode.OK, (await host.DeviceLoginAsync(code.Code)).StatusCode);

        await AssertProblemAsync(await host.DeviceLoginAsync(code.Code), HttpStatusCode.Unauthorized, AuthErrorCodes.InvalidLoginCode);
        await AssertProblemAsync(await host.DeviceLoginAsync("ABCDEFGH"), HttpStatusCode.Unauthorized, AuthErrorCodes.InvalidLoginCode);
    }

    [Fact]
    public async Task TheCodeOfADeviceOfAnotherAccountIsNotFoundAndNeedsThePanelCookie()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var ownerId = await host.RegisterAsync("auth-owner");
        await host.RegisterAsync("auth-other");
        var otherCookie = await host.SignInAccountAsync("auth-other");
        var device = await host.SignInDeviceAsync("auth-owner", ownerId);

        await AssertProblemAsync(await host.RequestLoginCodeAsync(otherCookie, ownerId), HttpStatusCode.NotFound, AuthErrorCodes.DeviceNotFound);
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.RequestLoginCodeAsync(null, ownerId)).StatusCode);

        // a device token is not the panel
        using var withDeviceToken = new HttpRequestMessage(HttpMethod.Post, AuthRoutes.DeviceLoginCode(ownerId));
        withDeviceToken.Headers.Authorization = new AuthenticationHeaderValue("Bearer", device.AccessToken);
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.Http.SendAsync(withDeviceToken, cancellationToken)).StatusCode);
    }

    [Fact]
    public async Task ThePanelCookieEndsWithItsSignOut()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var userId = await host.RegisterAsync("auth-panel-out");
        var cookie = await host.SignInAccountAsync("auth-panel-out");

        var logout = await host.SendAsPanelAsync(HttpMethod.Post, AuthRoutes.Logout, cookie);

        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        // the answer removes the cookie in the browser, and the server refuses the old one anyway
        Assert.Contains(logout.Headers.GetValues("Set-Cookie"), c => c.StartsWith(AccountCookie.CookieName + "=;"));
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.RequestLoginCodeAsync(cookie, userId)).StatusCode);
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
        var userId = await host.RegisterAsync("auth-rotation");
        var signIn = await host.SignInDeviceAsync("auth-rotation", userId);

        var refreshResponse = await RefreshAsync(host, signIn.RefreshToken);
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var refreshed = (await refreshResponse.Content.ReadFromJsonAsync<AuthTokensResponse>(cancellationToken))!;
        Assert.Equal(signIn.DeviceId, refreshed.DeviceId);
        Assert.NotEqual(signIn.RefreshToken, refreshed.RefreshToken);

        await AssertProblemAsync(await RefreshAsync(host, signIn.RefreshToken), HttpStatusCode.Unauthorized, AuthErrorCodes.RefreshReused);
        await AssertProblemAsync(await RefreshAsync(host, refreshed.RefreshToken), HttpStatusCode.Unauthorized, AuthErrorCodes.RefreshRevoked);
    }

    [Fact]
    public async Task ASignedOutDeviceCannotBeRefreshed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var userId = await host.RegisterAsync("auth-logout");
        var signIn = await host.SignInDeviceAsync("auth-logout", userId);

        using var logout = new HttpRequestMessage(HttpMethod.Post, AuthRoutes.Logout);
        logout.Headers.Authorization = new AuthenticationHeaderValue("Bearer", signIn.AccessToken);
        Assert.Equal(HttpStatusCode.NoContent, (await host.Http.SendAsync(logout, cancellationToken)).StatusCode);

        await AssertProblemAsync(await RefreshAsync(host, signIn.RefreshToken), HttpStatusCode.Unauthorized, AuthErrorCodes.RefreshRevoked);
    }

    // a code has about 40 bits, so guessing is slowed down per client address
    [Fact]
    public async Task DeviceLoginRefusesTooManyAttempts()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, deviceLoginPermitsPerMinute: 3);

        for (var i = 0; i < 3; i++)
            Assert.Equal(HttpStatusCode.Unauthorized, (await host.DeviceLoginAsync("ABCDEFGH")).StatusCode);

        Assert.Equal(HttpStatusCode.TooManyRequests, (await host.DeviceLoginAsync("ABCDEFGH")).StatusCode);
    }

    // the server closes the connection when the device token expires; the device refreshes and connects again
    [Fact]
    public async Task AnExpiredTokenClosesTheConnectionAndARefreshedOneConnectsAgain()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, accessTokenLifetime: TimeSpan.FromSeconds(2));
        var device = await host.ConnectAsync("auth-expiring", cancellationToken);
        var firstToken = device.Tokens.AccessToken;

        var closed = new TaskCompletionSource<Exception?>(TaskCreationOptions.RunContinuationsAsynchronously);
        device.Connection.Closed += exception =>
        {
            closed.TrySetResult(exception);
            return Task.CompletedTask;
        };

        await closed.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);

        await device.ReconnectAsync(cancellationToken);
        await device.Hub.ChangeName("Still Here");
        Assert.NotEqual(firstToken, device.Tokens.AccessToken);
    }

    private static async Task<DeviceLoginCodeResponse> IssueCodeAsync(AppHubTestHost host, string cookie, Guid deviceId)
    {
        var response = await host.RequestLoginCodeAsync(cookie, deviceId);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<DeviceLoginCodeResponse>())!;
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
