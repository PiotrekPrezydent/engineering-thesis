using System.Net;
using System.Net.Http.Headers;
using Dara.Server.Apps.API.Common.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Dara.Server.SystemTests.ClientServer;

// the policies apart from the endpoints of the panel and of clients: probes mapped by the
// test host; Client is the client token, Account the panel cookie
public class AuthPolicyTests
{
    private const string ClientProbe = "/probe/client";
    private const string AccountProbe = "/probe/account";
    private const string AnyProbe = "/probe/any";

    private static void MapProbes(WebApplication app)
    {
        app.MapGet(ClientProbe, () => Results.Ok()).RequireAuthorization(AuthPolicies.Client);
        app.MapGet(AccountProbe, () => Results.Ok()).RequireAuthorization(AuthPolicies.Account);
        // no policy of its own: the fallback policy needs a client token or the panel cookie
        app.MapGet(AnyProbe, () => Results.Ok());
    }

    [Fact]
    public async Task EachSessionPassesOnlyItsOwnPolicyAndBothPassTheFallback()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, mapExtra: MapProbes);
        await host.RegisterAsync("policy-user");
        var client = await host.RegisterClientAsync("policy-user");
        var cookie = await host.SignInAccountAsync("policy-user");

        Assert.Equal(HttpStatusCode.OK, await GetAsync(host, ClientProbe, client.AccessToken, null));
        Assert.Equal(HttpStatusCode.Unauthorized, await GetAsync(host, AccountProbe, client.AccessToken, null));
        Assert.Equal(HttpStatusCode.OK, await GetAsync(host, AccountProbe, null, cookie));
        Assert.Equal(HttpStatusCode.Unauthorized, await GetAsync(host, ClientProbe, null, cookie));
        Assert.Equal(HttpStatusCode.OK, await GetAsync(host, AnyProbe, client.AccessToken, null));
        Assert.Equal(HttpStatusCode.OK, await GetAsync(host, AnyProbe, null, cookie));
    }

    [Fact]
    public async Task NoTokenAndNoCookiePassNoPolicy()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, mapExtra: MapProbes);

        Assert.Equal(HttpStatusCode.Unauthorized, await GetAsync(host, ClientProbe, null, null));
        Assert.Equal(HttpStatusCode.Unauthorized, await GetAsync(host, AccountProbe, null, null));
        Assert.Equal(HttpStatusCode.Unauthorized, await GetAsync(host, AnyProbe, null, null));
    }

    private static async Task<HttpStatusCode> GetAsync(AppHubTestHost host, string path, string? accessToken, string? cookie)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        if (accessToken != null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        if (cookie != null)
            request.Headers.Add("Cookie", cookie);

        return (await host.Http.SendAsync(request)).StatusCode;
    }
}
