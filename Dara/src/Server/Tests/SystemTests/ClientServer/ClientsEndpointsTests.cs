using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dara.Shared.Contracts.Auth;
using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dara.Server.SystemTests.ClientServer;

// the account panel (its cookie) manages the clients of its account over http, a client (its token) only reads itself
public class ClientsEndpointsTests
{
    [Fact]
    public async Task ThePanelListsRenamesAndChangesTheContextOfItsClients()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var userId = await host.RegisterAsync("clients-owner");
        var phone = await host.RegisterClientAsync("clients-owner", "phone", " Bob-Phone ");
        var unnamed = await host.RegisterClientAsync("clients-owner", "watch");
        var cookie = await host.SignInAccountAsync("clients-owner");

        var all = await ReadAsync<List<AccountClientSnapshot>>(await host.SendAsPanelAsync(HttpMethod.Get, ClientRoutes.AccountClients, cookie));
        Assert.Equal([phone.ClientId, unnamed.ClientId], all.Select(c => c.ClientId));
        Assert.Equal((userId, "Bob-Phone", "Phone"), (all[0].UserId, all[0].Name, all[0].ClientType));
        Assert.Equal(("Watch 1", "Watch"), (all[1].Name, all[1].ClientType));

        Assert.Equal(HttpStatusCode.NoContent, (await host.SendAsPanelAsync(HttpMethod.Put, ClientRoutes.Name(unnamed.ClientId), cookie,
            new RenameClientRequest("Bob-Smartwatch"))).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await host.SendAsPanelAsync(HttpMethod.Put, ClientRoutes.Context(unnamed.ClientId), cookie,
            new ClientContextRequest(new Dictionary<string, string> { ["City"] = "Poznan" }))).StatusCode);

        var changed = await ReadAsync<AccountClientSnapshot>(await host.SendAsPanelAsync(HttpMethod.Get, ClientRoutes.Client(unnamed.ClientId), cookie));
        Assert.Equal("Bob-Smartwatch", changed.Name);
        Assert.Equal(new Dictionary<string, string> { ["City"] = "Poznan" }, changed.Context);

        var badName = await host.SendAsPanelAsync(HttpMethod.Put, ClientRoutes.Name(unnamed.ClientId), cookie, new RenameClientRequest(new string('x', 51)));
        await AssertProblemAsync(badName, HttpStatusCode.BadRequest, ClientErrorCodes.RuleBroken);
    }

    [Fact]
    public async Task AClientOfAnotherAccountIsNotFoundAndEachEndpointTakesItsOwnSession()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var client = await host.ConnectAsync("clients-victim", cancellationToken);
        await host.RegisterAsync("clients-stranger");
        var stranger = await host.SignInAccountAsync("clients-stranger");

        await AssertProblemAsync(await host.SendAsPanelAsync(HttpMethod.Get, ClientRoutes.Client(client.ClientId), stranger), HttpStatusCode.NotFound, ClientErrorCodes.ClientNotFound);
        await AssertProblemAsync(await host.SendAsPanelAsync(HttpMethod.Put, ClientRoutes.Name(client.ClientId), stranger, new RenameClientRequest("Mine")), HttpStatusCode.NotFound, ClientErrorCodes.ClientNotFound);
        await AssertProblemAsync(await host.SendAsPanelAsync(HttpMethod.Post, ClientRoutes.Revoke(client.ClientId), stranger), HttpStatusCode.NotFound, ClientErrorCodes.ClientNotFound);
        Assert.Empty(await ReadAsync<List<AccountClientSnapshot>>(await host.SendAsPanelAsync(HttpMethod.Get, ClientRoutes.AccountClients, stranger)));

        // the panel endpoints take the cookie only: a client token is not signed in there
        Assert.Equal(HttpStatusCode.Unauthorized, (await AsClientAsync(host, HttpMethod.Get, ClientRoutes.AccountClients, client.Tokens.AccessToken)).StatusCode);

        // and the client endpoint takes the token only
        Assert.Equal(HttpStatusCode.Unauthorized, (await host.SendAsPanelAsync(HttpMethod.Get, ClientRoutes.Current, stranger)).StatusCode);
    }

    [Fact]
    public async Task AnExistingClientSignsInWithALoginCodeAndReadsItself()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        await host.RegisterAsync("laptop-owner");
        var laptop = await host.RegisterClientAsync("laptop-owner", "pc", "Bob-Laptop");

        var tokens = await host.SignInClientAsync("laptop-owner", laptop.ClientId);
        Assert.Equal(laptop.ClientId, tokens.ClientId);

        var current = await ReadAsync<AccountClientSnapshot>(await AsClientAsync(host, HttpMethod.Get, ClientRoutes.Current, tokens.AccessToken));
        Assert.Equal((laptop.ClientId, "Bob-Laptop", "Pc"), (current.ClientId, current.Name, current.ClientType));
    }

    [Fact]
    public async Task ARevokedClientIsToldItsConnectionClosedAndItsTokenCodesAndChangesStopWorking()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var client = await host.ConnectAsync("lost-phone", cancellationToken);
        var cookie = await host.SignInAccountAsync("lost-phone");
        Assert.Equal(HttpStatusCode.OK, (await AsClientAsync(host, HttpMethod.Get, ClientRoutes.Current, client.Tokens.AccessToken)).StatusCode);

        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Connection.Closed += _ =>
        {
            closed.TrySetResult();
            return Task.CompletedTask;
        };

        Assert.Equal(HttpStatusCode.NoContent, (await host.SendAsPanelAsync(HttpMethod.Post, ClientRoutes.Revoke(client.ClientId), cookie)).StatusCode);

        await client.Events.WaitForAsync<ClientRevokedNotification>(n => n.ClientId == client.ClientId, "revoked");
        // the server closes the connection at once, the access token is still valid for minutes
        await closed.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
        Assert.Equal(HubConnectionState.Disconnected, client.Connection.State);

        Assert.Equal(HttpStatusCode.Unauthorized, (await AsClientAsync(host, HttpMethod.Get, ClientRoutes.Current, client.Tokens.AccessToken)).StatusCode);
        await AssertProblemAsync(await host.RequestLoginCodeAsync(cookie, client.ClientId), HttpStatusCode.Conflict, AuthErrorCodes.ClientRevoked);
        var refresh = await host.Http.PostAsJsonAsync(AuthRoutes.Refresh, new RefreshRequest(client.Tokens.RefreshToken), cancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
        await AssertProblemAsync(await host.SendAsPanelAsync(HttpMethod.Put, ClientRoutes.Name(client.ClientId), cookie, new RenameClientRequest("Found")),
            HttpStatusCode.Conflict, ClientErrorCodes.ClientRevoked);
    }

    private static async Task<HttpResponseMessage> AsClientAsync(AppHubTestHost host, HttpMethod method, string path, string accessToken)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await host.Http.SendAsync(request);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        Assert.True(response.IsSuccessStatusCode, $"{(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        Assert.Equal(status, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.Equal(code, problem!["code"].ToString());
    }
}
