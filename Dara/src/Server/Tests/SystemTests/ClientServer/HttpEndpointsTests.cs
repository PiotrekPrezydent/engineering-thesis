using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dara.Shared.Contracts.Account;
using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.FunctionCalls;
using Dara.Shared.Contracts.Groups;
using Dara.Shared.Contracts.Orchestration;
using Dara.Shared.Contracts.Plugins;

namespace Dara.Server.SystemTests.ClientServer;

// every endpoint of TASK-ENG-032 takes only its own kind of session: a client route only the client token, a panel route
// only the panel cookie, both answer 401 without a redirect otherwise; the panel acts only on clients of its account
public class HttpEndpointsTests
{
    private static readonly Guid AnyId = Guid.NewGuid();

    private static readonly (HttpMethod Method, string Path)[] ClientRoutes =
    [
        (HttpMethod.Post, GroupRoutes.Messages(AnyId)),
        (HttpMethod.Get, AgentRunRoutes.AgentRun(AnyId)),
        (HttpMethod.Post, AgentRunRoutes.Approve(AnyId)),
        (HttpMethod.Post, AgentRunRoutes.Reject(AnyId)),
        (HttpMethod.Get, PluginRoutes.CurrentClientPlugins),
        (HttpMethod.Put, PluginRoutes.CurrentClientPlugins),
        (HttpMethod.Post, FunctionCallRoutes.Result(AnyId))
    ];

    private static readonly (HttpMethod Method, string Path)[] PanelRoutes =
    [
        (HttpMethod.Get, AccountRoutes.Antiforgery),
        (HttpMethod.Post, GroupRoutes.AccountGroups),
        (HttpMethod.Post, GroupRoutes.Join(AnyId)),
        (HttpMethod.Delete, GroupRoutes.Membership(AnyId, AnyId)),
        (HttpMethod.Get, AgentRunRoutes.AgentSettings(AnyId)),
        (HttpMethod.Put, AgentRunRoutes.PromptPrefix(AnyId)),
        (HttpMethod.Put, AgentRunRoutes.Provider(AnyId)),
        (HttpMethod.Put, AgentRunRoutes.Model(AnyId)),
        (HttpMethod.Put, AgentRunRoutes.ApiKey(AnyId)),
        (HttpMethod.Get, PluginRoutes.ClientPlugins(AnyId)),
        (HttpMethod.Post, PluginRoutes.EnablePlugin(AnyId, AnyId)),
        (HttpMethod.Post, PluginRoutes.DisablePlugin(AnyId, AnyId)),
        (HttpMethod.Post, PluginRoutes.EnableFunction(AnyId, AnyId, AnyId)),
        (HttpMethod.Post, PluginRoutes.DisableFunction(AnyId, AnyId, AnyId))
    ];

    [Fact]
    public async Task EveryRouteTakesOnlyItsOwnSessionAndNeverRedirects()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var client = await host.ConnectAsync("routes-client", cancellationToken);
        var cookie = await host.SignInAccountAsync(client.Identifier);
        var token = await client.GetAccessTokenAsync();

        foreach (var (method, path) in ClientRoutes.Concat(PanelRoutes))
            await AssertUnauthorizedAsync(await SendAsync(host, method, path, null, null), $"{method} {path} without a session");
        foreach (var (method, path) in ClientRoutes)
            await AssertUnauthorizedAsync(await SendAsync(host, method, path, null, cookie), $"{method} {path} with the panel cookie");
        foreach (var (method, path) in PanelRoutes)
            await AssertUnauthorizedAsync(await SendAsync(host, method, path, token, null), $"{method} {path} with a client token");
    }

    [Fact]
    public async Task ThePanelGetsAnAntiforgeryTokenForItsForms()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var client = await host.ConnectAsync("forms-client", cancellationToken);
        var panel = await host.PanelOfAsync(client);

        var response = await panel.SendAsync(HttpMethod.Get, AccountRoutes.Antiforgery);

        await AppHubTestHost.EnsureAsync(response, "antiforgery token");
        var token = (await response.Content.ReadFromJsonAsync<AntiforgeryTokenResponse>(cancellationToken))!;
        Assert.False(string.IsNullOrWhiteSpace(token.RequestToken));
        Assert.False(string.IsNullOrWhiteSpace(token.FormFieldName));
    }

    [Fact]
    public async Task ThePanelActsOnlyOnLiveClientsOfItsAccount()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var mine = await host.ConnectAsync("panel-owner", cancellationToken);
        var other = await host.ConnectAsync("panel-stranger", cancellationToken);
        var spare = await host.ConnectAnotherClientAsync(mine, cancellationToken);
        var panel = await host.PanelOfAsync(mine);
        var group = await host.CreateGroupAsync(other, "foreign");

        var foreignClient = await PanelTestClient.ProblemAsync(await panel.GetPluginsAsync(other.ClientId));
        var foreignOwner = await PanelTestClient.ProblemAsync(await panel.SendAsync(HttpMethod.Post, GroupRoutes.AccountGroups,
            new CreateGroupRequest("not mine", other.ClientId)));
        var foreignGroup = await PanelTestClient.ProblemAsync(await panel.GetAgentSettingsAsync(group.GroupId));
        var missingGroup = await PanelTestClient.ProblemAsync(await panel.GetAgentSettingsAsync(Guid.NewGuid()));

        await AppHubTestHost.EnsureAsync(await panel.SendAsync(HttpMethod.Post, Shared.Contracts.Clients.ClientRoutes.Revoke(spare.ClientId)), "revoke");
        var revoked = await PanelTestClient.ProblemAsync(await panel.JoinGroupAsync(spare.ClientId, group.GroupId, group.JoinCode));
        var twice = await PanelTestClient.ProblemAsync(await panel.LeaveGroupAsync(mine.ClientId, group.GroupId));

        Assert.Equal((404, ClientErrorCodes.ClientNotFound), (foreignClient.Status, foreignClient.Code));
        Assert.Equal((404, ClientErrorCodes.ClientNotFound), (foreignOwner.Status, foreignOwner.Code));
        Assert.Equal((404, "not_found"), (foreignGroup.Status, foreignGroup.Code));
        Assert.Equal((404, "not_found"), (missingGroup.Status, missingGroup.Code));
        Assert.Equal((409, ClientErrorCodes.ClientRevoked), (revoked.Status, revoked.Code));
        Assert.Equal((409, "conflict", "NonActualMemberCannotLeaveGroupRule"), (twice.Status, twice.Code, twice.Rule));
    }

    [Fact]
    public async Task OnlyAMemberWritesInAGroupAndAnUnknownRunIsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var owner = await host.ConnectAsync("writing-owner", cancellationToken);
        var outsider = await host.ConnectAsync("writing-outsider", cancellationToken);
        var group = await host.CreateGroupAsync(owner, "writers");

        var notMember = await SendAsync(host, HttpMethod.Post, GroupRoutes.Messages(group.GroupId), await outsider.GetAccessTokenAsync(), null,
            new SendGroupMessageRequest("let me in"));
        var noGroup = await SendAsync(host, HttpMethod.Post, GroupRoutes.Messages(Guid.NewGuid()), await owner.GetAccessTokenAsync(), null,
            new SendGroupMessageRequest("anyone?"));

        var forbidden = await PanelTestClient.ProblemAsync(notMember);
        var missing = await PanelTestClient.ProblemAsync(noGroup);
        Assert.Equal((403, "forbidden", "OnlyActualGroupMemberCanSendMessageRule"), (forbidden.Status, forbidden.Code, forbidden.Rule));
        Assert.Equal((404, "not_found"), (missing.Status, missing.Code));
        Assert.Null(await owner.Api.GetAgentRunAsync(Guid.NewGuid(), cancellationToken));
    }

    private static async Task<HttpResponseMessage> SendAsync(AppHubTestHost host, HttpMethod method, string path, string? token, string? cookie,
        object? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        if (body != null)
            request.Content = JsonContent.Create(body);
        else if (method != HttpMethod.Get && method != HttpMethod.Delete)
            request.Content = JsonContent.Create(new { });
        if (token != null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (cookie != null)
            request.Headers.Add("Cookie", cookie);

        return await host.Http.SendAsync(request);
    }

    private static Task AssertUnauthorizedAsync(HttpResponseMessage response, string what)
    {
        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, $"{what}: {(int)response.StatusCode}");
        Assert.Null(response.Headers.Location);
        return Task.CompletedTask;
    }
}
