using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Authorization;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.GetMyAssignments;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts.Auth;
using Dara.Shared.Contracts.Authorization;
using Dara.Shared.Contracts.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dara.Server.SystemTests.ClientServer;

// the mechanism of TASK-ENG-040 on probes of the test host (endpoints and hub methods marked with permissions of
// Identity, for any session) and the endpoints of Authorization; the sender is the client of a token or the user of
// the panel cookie, each with only its own roles
public class AuthorizationEndpointsTests
{
    private const string ClientProbe = "/test/clients/{clientId}/change-context";
    private const string AccountProbe = "/test/users/{userId}/read-clients";
    private const string BothProbe = "/test/users/{userId}/clients/{clientId}";
    private const string ProbeHubPath = "/test/probe-hub";

    private static void MapProbes(WebApplication app)
    {
        app.MapGet(ClientProbe, () => Results.Ok()).RequirePermission(IdentityPermissions.ChangeClientContext, "clientId");
        app.MapGet(AccountProbe, () => Results.Ok()).RequirePermission(IdentityPermissions.ReadClients, "userId");
        app.MapGet(BothProbe, () => Results.Ok())
            .RequirePermission(IdentityPermissions.ReadClients, "userId")
            .RequirePermission(IdentityPermissions.ChangeClientContext, "clientId");
        app.MapHub<ProbeHub>(ProbeHubPath).RequireAuthorization(AuthPolicies.Client);
    }

    [Fact]
    public async Task ANewAccountAndClientHoldThemselvesAndTheRolesTheModulesGiveNewSubjects()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, mapExtra: MapProbes);
        var alice = await NewClientAsync(host, "authz-alice");
        var panel = await host.SignInAccountAsync("authz-alice");

        var ofClient = await GetAsync<List<RoleAssignmentSnapshot>>(host, AuthorizationRoutes.MyAssignments, alice.AccessToken);
        Assert.Contains(ofClient, a => a is { RoleCode: "Identity.ClientOwner", ScopeCode: "Identity.Client" } && a.ScopeId == alice.ClientId);
        Assert.Contains(ofClient, a => a is { RoleCode: "Plugins.Viewer", ScopeCode: "Plugins.Plugin" } && a.ScopeId == Guid.Empty);
        // a client does not get the roles of its user
        Assert.DoesNotContain(ofClient, a => a.RoleCode == "Identity.Owner");
        Assert.All(ofClient, a => Assert.Equal(alice.ClientId, a.SubjectId));

        var ofUser = await ReadAsync<List<RoleAssignmentSnapshot>>(await host.SendAsPanelAsync(HttpMethod.Get, AuthorizationRoutes.MyAssignments, panel));
        Assert.Contains(ofUser, a => a is { RoleCode: "Identity.Owner", ScopeCode: "Identity.User" } && a.ScopeId == alice.UserId);
        Assert.Contains(ofUser, a => a.RoleCode == "Identity.ClientOwner" && a.ScopeId == alice.ClientId);
        Assert.Contains(ofUser, a => a.RoleCode == "Plugins.Viewer" && a.ScopeId == Guid.Empty);

        var roles = await GetAsync<List<RoleSnapshot>>(host, AuthorizationRoutes.Roles, alice.AccessToken);
        var pluginsOwner = Assert.Single(roles, r => r.Code == "Plugins.Owner");
        Assert.Equal(["plugins.plugin.configure", "plugins.plugin.manage_access", "plugins.plugin.read"], pluginsOwner.PermissionCodes);
        // the function owner holds on each function id, so a grant of the plugin owner passes it along (a companion)
        Assert.Equal(["Plugins.FunctionOwner"], pluginsOwner.CompanionRoleCodes);
        Assert.Contains("Plugins.Owner", pluginsOwner.ManageableRoleCodes);
        Assert.DoesNotContain("Plugins.Owner", roles.Single(r => r.Code == "Plugins.Manager").ManageableRoleCodes);
        Assert.Contains(roles, r => r is { Code: "Authorization.ServerAdmin", ScopeCode: "Authorization.Server" });
    }

    [Fact]
    public async Task AMarkedEndpointAllowsOnlyWhatTheOwnAssignmentsOfTheSenderAllow()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, mapExtra: MapProbes);
        var alice = await NewClientAsync(host, "authz-owner");
        var bob = await NewClientAsync(host, "authz-guest");
        var panel = await host.SignInAccountAsync("authz-owner");
        var clientProbe = ClientProbe.Replace("{clientId}", alice.ClientId.ToString());
        var accountProbe = AccountProbe.Replace("{userId}", alice.UserId.ToString());
        var bothProbe = BothProbe.Replace("{userId}", alice.UserId.ToString()).Replace("{clientId}", alice.ClientId.ToString());

        Assert.Equal(HttpStatusCode.OK, (await SendAsync(host, HttpMethod.Get, clientProbe, alice.AccessToken)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await host.SendAsPanelAsync(HttpMethod.Get, clientProbe, panel)).StatusCode);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Get, clientProbe, bob.AccessToken), HttpStatusCode.Forbidden, AuthorizationErrorCodes.Forbidden);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Get, ClientProbe.Replace("{clientId}", "not-a-guid"), alice.AccessToken),
            HttpStatusCode.BadRequest, AuthorizationErrorCodes.InvalidScopeId);

        // the account is the resource of the user: its panel may read the clients, its client may not
        Assert.Equal(HttpStatusCode.OK, (await host.SendAsPanelAsync(HttpMethod.Get, accountProbe, panel)).StatusCode);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Get, accountProbe, alice.AccessToken), HttpStatusCode.Forbidden, AuthorizationErrorCodes.Forbidden);

        // two marks: both are checked
        Assert.Equal(HttpStatusCode.OK, (await host.SendAsPanelAsync(HttpMethod.Get, bothProbe, panel)).StatusCode);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Get, bothProbe, alice.AccessToken), HttpStatusCode.Forbidden, AuthorizationErrorCodes.Forbidden);
    }

    [Fact]
    public async Task GrantsAndRevokesFollowTheRolesOfTheSender()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, mapExtra: MapProbes);
        var alice = await NewClientAsync(host, "authz-sharer");
        var bob = await NewClientAsync(host, "authz-receiver");
        var panel = await host.SignInAccountAsync("authz-sharer");
        var clientProbe = ClientProbe.Replace("{clientId}", alice.ClientId.ToString());
        var accountProbe = AccountProbe.Replace("{userId}", alice.UserId.ToString());
        var holdAlicesClient = new RoleAssignmentRequest(bob.ClientId, "Identity.ClientOwner", alice.ClientId);
        var viewAlicesAccount = new RoleAssignmentRequest(bob.ClientId, "Identity.Viewer", alice.UserId);

        // bob manages no access to the client of alice, so he cannot give himself a role there
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Post, AuthorizationRoutes.Assignments, bob.AccessToken, holdAlicesClient),
            HttpStatusCode.Forbidden, AuthorizationErrorCodes.Forbidden);

        Assert.Equal(HttpStatusCode.NoContent, (await SendAsync(host, HttpMethod.Post, AuthorizationRoutes.Assignments, alice.AccessToken, holdAlicesClient)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await SendAsync(host, HttpMethod.Get, clientProbe, bob.AccessToken)).StatusCode);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Post, AuthorizationRoutes.Assignments, alice.AccessToken, holdAlicesClient),
            HttpStatusCode.Conflict, AuthorizationErrorCodes.AlreadyAssigned);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Post, AuthorizationRoutes.Assignments, alice.AccessToken, holdAlicesClient with { RoleCode = "Identity.Nobody" }),
            HttpStatusCode.NotFound, AuthorizationErrorCodes.RoleNotFound);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Post, AuthorizationRoutes.Assignments, alice.AccessToken,
                new RoleAssignmentRequest(bob.ClientId, "Plugins.Viewer", Guid.Empty)),
            HttpStatusCode.BadRequest, AuthorizationErrorCodes.GlobalAssignment);

        // the panel of alice manages her account and lets the client of bob see it; a viewer manages no access there
        Assert.Equal(HttpStatusCode.NoContent, (await host.SendAsPanelAsync(HttpMethod.Post, AuthorizationRoutes.Assignments, panel, viewAlicesAccount)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await SendAsync(host, HttpMethod.Get, accountProbe, bob.AccessToken)).StatusCode);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Post, AuthorizationRoutes.Assignments, bob.AccessToken, viewAlicesAccount with { RoleCode = "Identity.Owner" }),
            HttpStatusCode.Forbidden, AuthorizationErrorCodes.Forbidden);

        Assert.Equal(HttpStatusCode.NoContent, (await SendAsync(host, HttpMethod.Post, AuthorizationRoutes.Revoke, alice.AccessToken, holdAlicesClient)).StatusCode);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Get, clientProbe, bob.AccessToken), HttpStatusCode.Forbidden, AuthorizationErrorCodes.Forbidden);
        await AssertProblemAsync(await SendAsync(host, HttpMethod.Post, AuthorizationRoutes.Revoke, alice.AccessToken, holdAlicesClient),
            HttpStatusCode.NotFound, AuthorizationErrorCodes.AssignmentNotFound);
    }

    [Fact]
    public async Task AMarkedHubMethodChecksEveryMark()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, mapExtra: MapProbes);
        var alice = await NewClientAsync(host, "authz-hub-owner");
        var bob = await NewClientAsync(host, "authz-hub-guest");

        await using var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(host.Http.BaseAddress!, ProbeHubPath), options => options.AccessTokenProvider = () => Task.FromResult<string?>(alice.AccessToken))
            .Build();
        await connection.StartAsync(cancellationToken);

        Assert.Equal("ok", await connection.InvokeAsync<string>(nameof(ProbeHub.ChangeContext), alice.ClientId, cancellationToken));
        var foreign = await Assert.ThrowsAsync<HubException>(() => connection.InvokeAsync<string>(nameof(ProbeHub.ChangeContext), bob.ClientId, cancellationToken));
        Assert.Contains(AuthorizationErrorCodes.Forbidden, foreign.Message);

        // the client holds itself, not the account of its user
        var second = await Assert.ThrowsAsync<HubException>(() =>
            connection.InvokeAsync<string>(nameof(ProbeHub.ReadClientsOf), alice.UserId, alice.ClientId, cancellationToken));
        Assert.Contains(AuthorizationErrorCodes.Forbidden, second.Message);
    }

    [Fact]
    public async Task ARevokedClientLosesItsAssignments()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken, mapExtra: MapProbes);
        var alice = await NewClientAsync(host, "authz-revoked");
        var cookie = await host.SignInAccountAsync("authz-revoked");

        var revoked = await host.SendAsPanelAsync(HttpMethod.Post, ClientRoutes.Revoke(alice.ClientId), cookie);
        Assert.True(revoked.IsSuccessStatusCode, $"revoke: {(int)revoked.StatusCode}");

        await ModulesHost.WaitUntilAsync(async () => (await AssignmentsAsync(host, alice.ClientId)).Count == 0
                && (await AssignmentsAsync(host, alice.UserId)).All(a => a.ScopeId != alice.ClientId),
            "the assignments of and on the revoked client are gone");
        Assert.Contains(await AssignmentsAsync(host, alice.UserId), a => a.RoleCode == "Identity.Owner");
    }

    // an account with one client whose assignments from Identity and Plugins have arrived in Authorization: the client
    // holds itself and sees the plugins, the user holds its account and the client and sees the plugins
    private static async Task<AuthTokensResponse> NewClientAsync(AppHubTestHost host, string login)
    {
        await host.RegisterAsync(login);
        var tokens = await host.RegisterClientAsync(login);
        await ModulesHost.WaitUntilAsync(async () => (await AssignmentsAsync(host, tokens.ClientId)).Count >= 2
                && (await AssignmentsAsync(host, tokens.UserId)).Count >= 3,
            $"the assignments of {login}");
        return tokens;
    }

    private static Task<List<RoleAssignmentDto>> AssignmentsAsync(AppHubTestHost host, Guid subjectId)
    {
        return host.QueryAsync<IAuthorizationModule, List<RoleAssignmentDto>>(module =>
            module.ExecuteQueryAsync<GetMyAssignmentsQuery, List<RoleAssignmentDto>>(new GetMyAssignmentsQuery(subjectId)));
    }

    private static async Task<HttpResponseMessage> SendAsync(AppHubTestHost host, HttpMethod method, string path, string accessToken, object? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        if (body != null)
            request.Content = JsonContent.Create(body);

        return await host.Http.SendAsync(request);
    }

    private static async Task<T> GetAsync<T>(AppHubTestHost host, string path, string accessToken)
    {
        return await ReadAsync<T>(await SendAsync(host, HttpMethod.Get, path, accessToken));
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        Assert.Equal(status, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<Dictionary<string, object?>>();
        Assert.Equal(code, problem!["code"]?.ToString());
    }
}

// a hub of the tests only, so no method of AppHub is marked (TASK-ENG-040-D08)
public class ProbeHub : Hub
{
    [RequirePermission(typeof(IdentityPermissions), nameof(IdentityPermissions.ChangeClientContext), ScopeIdArgument = "clientId")]
    public string ChangeContext(Guid clientId) => "ok";

    [RequirePermission(typeof(IdentityPermissions), nameof(IdentityPermissions.ChangeClientContext), ScopeIdArgument = "clientId")]
    [RequirePermission(typeof(IdentityPermissions), nameof(IdentityPermissions.ReadClients), ScopeIdArgument = "userId")]
    public string ReadClientsOf(Guid userId, Guid clientId) => "ok";
}
