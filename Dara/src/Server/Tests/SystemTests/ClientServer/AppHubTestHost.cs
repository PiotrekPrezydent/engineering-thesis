using System.Net.Http.Json;
using System.Security.Cryptography;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.Modules.Groups.Domain.Members;
using Dara.Server.Modules.Authorization.Infrastructure;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;
using Dara.Server.Modules.Identity.Application.RegisterUser;
using Dara.Shared.Contracts.Auth;
using Dara.Shared.Contracts.Clients;
using Dara.Server.Modules.Identity.Infrastructure;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Infrastructure;
using Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;
using Dara.Server.Modules.Orchestration.IntegrationTests.Fakes;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Infrastructure;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ApiProgram = Dara.Server.Apps.API.Program;

namespace Dara.Server.SystemTests.ClientServer;

// the api host of the app on a free loopback port, with a scripted model; clients call http and listen on the hub
// like the cli, the panel of an account calls http with its cookie
public class AppHubTestHost : IAsyncDisposable
{
    private readonly WebApplication _app;
    private readonly List<HubTestClient> _clients = new();

    public IdentityCompositionRoot IdentityRoot { get; } = new() { ProcessingOptions = ModulesHost.ProcessingOptions };
    public AuthorizationCompositionRoot AuthorizationRoot { get; } = new() { ProcessingOptions = ModulesHost.ProcessingOptions };
    public GroupsCompositionRoot GroupsRoot { get; } = new() { ProcessingOptions = ModulesHost.ProcessingOptions };
    public PluginsCompositionRoot PluginsRoot { get; } = new() { ProcessingOptions = ModulesHost.ProcessingOptions };
    public OrchestrationCompositionRoot OrchestrationRoot { get; }

    // no test calls the api of a model provider, every plan comes from this script
    public ScriptedChatClientProvider Model { get; } = new();

    public string HubUrl { get; private set; } = "";

    // the http api of the host, with the base address of the host; it keeps no cookies: the panel cookie is Secure,
    // so over the http of the tests it is sent by hand (SendAsPanelAsync)
    public HttpClient Http { get; } = new(new SocketsHttpHandler { UseCookies = false });

    // the key of this host only, random per run, never in the repository
    public string SigningKey { get; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));

    private AppHubTestHost(TimeSpan? accessTokenLifetime, Action<WebApplication>? mapExtra, int? clientCodePermitsPerMinute)
    {
        OrchestrationRoot = new OrchestrationCompositionRoot([Model], new ExecutionPlanGeneratorOptions(TimeoutSeconds: 5)) { ProcessingOptions = ModulesHost.ProcessingOptions };

        // same order as in the api host, the seed is off so tests see only their own data
        IModuleCompositionRoot[] roots = [IdentityRoot, AuthorizationRoot, GroupsRoot, PluginsRoot, OrchestrationRoot];
        List<string> args =
        [
            "--urls", "http://127.0.0.1:0", "--Seed:Enabled", "false", "--Logging:LogLevel:Default", "Warning",
            "--Jwt:SigningKey", SigningKey
        ];
        if (accessTokenLifetime is { } lifetime)
            args.AddRange(["--Jwt:AccessTokenLifetime", lifetime.ToString("c")]);
        if (clientCodePermitsPerMinute is { } permits)
            args.AddRange(["--Auth:ClientCodePermitsPerMinute", permits.ToString()]);

        _app = ApiProgram.CreateApplication(args.ToArray(), roots, mapExtra);
    }

    public static async Task<AppHubTestHost> StartAsync(CancellationToken cancellationToken, TimeSpan? accessTokenLifetime = null,
        Action<WebApplication>? mapExtra = null, int? clientCodePermitsPerMinute = null)
    {
        var host = new AppHubTestHost(accessTokenLifetime, mapExtra, clientCodePermitsPerMinute);
        await host._app.StartAsync(cancellationToken);

        // kestrel puts the port it got for port 0 into the addresses
        var baseUrl = host._app.Urls.First();
        host.HubUrl = $"{baseUrl}/{Connections.HubName}";
        host.Http.BaseAddress = new Uri(baseUrl);
        return host;
    }

    // registers an account and one client of it like the app does (a registration code of the panel) and connects
    // the client; ready when every module knows the client, like a client that waits for the seed
    public async Task<HubTestClient> ConnectAsync(string login, CancellationToken cancellationToken)
    {
        await RegisterAsync(login);
        var tokens = await RegisterClientAsync(login);

        var client = new HubTestClient(HubUrl, Http, login, tokens);
        _clients.Add(client);
        await client.StartAsync(cancellationToken);

        await WaitForClientAsync(client.ClientId);

        return client;
    }

    // the panel of the owner creates the group with the owner client, the panel of each member joins it with its
    // code; ready when orchestration has them all
    public async Task<GroupCreatedNotification> CreateGroupAsync(HubTestClient owner, string groupName, params HubTestClient[] members)
    {
        var ownerPanel = await PanelOfAsync(owner);
        await ownerPanel.CreateGroupAsync(groupName, owner.ClientId);
        var group = await owner.Events.WaitForAsync<GroupCreatedNotification>(g => g.Name == groupName, $"group {groupName} created");

        foreach (var member in members)
        {
            var memberPanel = await PanelOfAsync(member);
            await EnsureAsync(await memberPanel.JoinGroupAsync(member.ClientId, group.GroupId, group.JoinCode), $"{member.Identifier} joins {groupName}");
            await member.Events.WaitForAsync<GroupSnapshot>(g => g.GroupId == group.GroupId, $"{member.Identifier} joined {groupName}");
        }

        foreach (var client in members.Prepend(owner))
            await ModulesHost.WaitUntilAsync(async () => await QueryAsync<IOrchestrationModule, GroupAgentSettingsDto?>(module =>
                    module.ExecuteQueryAsync<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>(new GetGroupAgentSettingsQuery(group.GroupId, client.ClientId))) is not null,
                $"{client.Identifier} in the orchestration group {groupName}");

        return group;
    }

    // the panel of the account of the client (every test client has an account of its own, named by its identifier)
    public async Task<PanelTestClient> PanelOfAsync(HubTestClient client)
    {
        return new PanelTestClient(this, await SignInAccountAsync(client.Identifier));
    }

    // the alias of a tool comes from the participant name, so a plan needs the name and the functions in orchestration
    public async Task WaitForParticipantAsync(Guid clientId, string name, params string[] functionNames)
    {
        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(OrchestrationRoot,
            model => model.Query<Participant>().AnyAsync(p => p.Id.Match(clientId) && p.Name == name)), $"participant {name}");

        await ModulesHost.WaitUntilAsync(async () =>
        {
            var functions = await GetParticipantFunctionNamesAsync(clientId);
            return functionNames.All(functions.Contains);
        }, $"functions of {name}");
    }

    // names of the functions orchestration can plan with, sorted
    public async Task<IReadOnlyList<string>> GetParticipantFunctionNamesAsync(Guid clientId)
    {
        var functions = await QueryAsync<IOrchestrationModule, List<ParticipantFunctionDto>>(module =>
            module.ExecuteQueryAsync<GetParticipantFunctionsQuery, List<ParticipantFunctionDto>>(new GetParticipantFunctionsQuery(clientId)));

        return functions.Select(f => f.Name).Order().ToList();
    }

    public async Task<TResult> QueryAsync<TModule, TResult>(Func<TModule, Task<TResult>> query) where TModule : notnull
    {
        await using var scope = _app.Services.CreateAsyncScope();
        return await query(scope.ServiceProvider.GetRequiredService<TModule>());
    }

    // there is no registration endpoint for accounts: an account without clients, through the module like the seed;
    // the e-mail and the password of every test account follow from the login
    public Task<Guid> RegisterAsync(string login)
    {
        return QueryAsync<IIdentityModule, Guid>(module => module.ExecuteCommandAsync<RegisterUserCommand, Guid>(
            new RegisterUserCommand(login, $"{login}@test.local", ModulesHost.TestPassword)));
    }

    public Task<HttpResponseMessage> LoginAsync(LoginRequest request)
    {
        return Http.PostAsJsonAsync(AuthRoutes.Login, request);
    }

    // the account panel: the value of its session cookie ("dara.account=..."), never connected to the hub
    public async Task<string> SignInAccountAsync(string login)
    {
        var response = await LoginAsync(new LoginRequest(login, ModulesHost.TestPassword));
        await EnsureAsync(response, $"account sign-in of {login}");
        return SessionCookie(response) ?? throw new InvalidOperationException($"account sign-in of {login} set no cookie");
    }

    // the cookie of the answer as the browser sends it back, without its attributes
    public static string? SessionCookie(HttpResponseMessage response)
    {
        return response.Headers.TryGetValues("Set-Cookie", out var cookies)
            ? cookies.Select(c => c.Split(';')[0]).FirstOrDefault(c => c.StartsWith(AccountCookie.CookieName + "=") && c.Length > AccountCookie.CookieName.Length + 1)
            : null;
    }

    // a request of the panel with its cookie
    public async Task<HttpResponseMessage> SendAsPanelAsync(HttpMethod method, string path, string? cookie, object? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        if (body != null)
            request.Content = JsonContent.Create(body);
        if (cookie != null)
            request.Headers.Add("Cookie", cookie);

        return await Http.SendAsync(request);
    }

    public Task<HttpResponseMessage> RequestRegistrationCodeAsync(string? cookie)
    {
        return SendAsPanelAsync(HttpMethod.Post, AuthRoutes.RegistrationCode, cookie);
    }

    public Task<HttpResponseMessage> RequestLoginCodeAsync(string? cookie, Guid clientId)
    {
        return SendAsPanelAsync(HttpMethod.Post, AuthRoutes.ClientLoginCode(clientId), cookie);
    }

    // the account panel of the client renames it (the name is read-only in the app of the client)
    public async Task RenameClientAsync(HubTestClient client, string name)
    {
        var cookie = await SignInAccountAsync(client.Identifier);
        await EnsureAsync(await SendAsPanelAsync(HttpMethod.Put, ClientRoutes.Name(client.ClientId), cookie, new RenameClientRequest(name)),
            $"rename of {client.Identifier}");
    }

    public Task<HttpResponseMessage> RegisterClientWithCodeAsync(string code, string? type = "Pc", string? name = null)
    {
        return Http.PostAsJsonAsync(AuthRoutes.RegisterClient, new RegisterClientRequest(code, type, name));
    }

    public Task<HttpResponseMessage> ClientLoginAsync(string code)
    {
        return Http.PostAsJsonAsync(AuthRoutes.ClientLogin, new ClientLoginRequest(code));
    }

    // a new client of the account: the panel signs in and creates a registration code, the app registers with it
    public async Task<AuthTokensResponse> RegisterClientAsync(string login, string? type = "Pc", string? name = null)
    {
        var cookie = await SignInAccountAsync(login);

        var codeResponse = await RequestRegistrationCodeAsync(cookie);
        await EnsureAsync(codeResponse, $"registration code of {login}");
        var code = (await codeResponse.Content.ReadFromJsonAsync<ClientCodeResponse>())!;

        var response = await RegisterClientWithCodeAsync(code.Code, type, name);
        await EnsureAsync(response, $"client registration of {login}");
        return (await response.Content.ReadFromJsonAsync<AuthTokensResponse>())!;
    }

    // an existing client signs in again: the panel issues a login code for it and the client signs in with it
    public async Task<AuthTokensResponse> SignInClientAsync(string login, Guid clientId)
    {
        var cookie = await SignInAccountAsync(login);

        var codeResponse = await RequestLoginCodeAsync(cookie, clientId);
        await EnsureAsync(codeResponse, $"login code of {login}");
        var code = (await codeResponse.Content.ReadFromJsonAsync<ClientCodeResponse>())!;

        var response = await ClientLoginAsync(code.Code);
        await EnsureAsync(response, $"client sign-in of {login}");
        return (await response.Content.ReadFromJsonAsync<AuthTokensResponse>())!;
    }

    public static async Task EnsureAsync(HttpResponseMessage response, string what)
    {
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"{what} failed: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
    }

    private async Task WaitForClientAsync(Guid clientId)
    {
        var clients = await QueryAsync<IIdentityModule, List<ClientDto>>(module =>
            module.ExecuteQueryAsync<GetClientsQuery, List<ClientDto>>(new GetClientsQuery([clientId])));
        Assert.Single(clients);

        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(GroupsRoot,
            model => model.Query<Member>().AnyAsync(m => m.Id.Match(clientId))), "groups member of the client");
        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(PluginsRoot,
            model => model.Query<PluginOwner>().AnyAsync(o => o.Id.Match(clientId))), "plugin owner of the client");
        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(OrchestrationRoot,
            model => model.Query<Participant>().AnyAsync(p => p.Id.Match(clientId))), "participant of the client");
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var client in _clients)
            await client.DisposeAsync();

        await _app.StopAsync();
        await _app.DisposeAsync();
        Http.Dispose();
    }
}
