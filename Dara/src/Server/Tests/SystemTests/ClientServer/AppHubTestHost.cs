using System.Net.Http.Json;
using System.Security.Cryptography;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.Modules.Groups.Domain.Members;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Apps.API.Authentication;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.RegisterDeviceSession;
using Dara.Server.Modules.Identity.Application.RegisterUser;
using Dara.Shared.Contracts.Auth;
using Dara.Server.Modules.Identity.Infrastructure;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Infrastructure;
using Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;
using Dara.Server.Modules.Orchestration.IntegrationTests.Fakes;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Infrastructure;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.GetProfile;
using Dara.Server.Modules.Profiles.Infrastructure;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ApiProgram = Dara.Server.Apps.API.Program;

namespace Dara.Server.SystemTests.ClientServer;

// the api host of the app on a free loopback port, with a scripted model; clients connect over signalr like the cli
public class AppHubTestHost : IAsyncDisposable
{
    private readonly WebApplication _app;
    private readonly List<HubTestClient> _clients = new();

    public IdentityCompositionRoot IdentityRoot { get; } = new() { ProcessingOptions = ModulesHost.ProcessingOptions };
    public ProfilesCompositionRoot ProfilesRoot { get; } = new() { ProcessingOptions = ModulesHost.ProcessingOptions };
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

    private AppHubTestHost(TimeSpan? accessTokenLifetime, Action<WebApplication>? mapExtra, int? deviceLoginPermitsPerMinute)
    {
        OrchestrationRoot = new OrchestrationCompositionRoot([Model], new ExecutionPlanGeneratorOptions(TimeoutSeconds: 5)) { ProcessingOptions = ModulesHost.ProcessingOptions };

        // same order as in the api host, the seed is off so tests see only their own data
        IModuleCompositionRoot[] roots = [IdentityRoot, GroupsRoot, ProfilesRoot, PluginsRoot, OrchestrationRoot];
        List<string> args =
        [
            "--urls", "http://127.0.0.1:0", "--Seed:Enabled", "false", "--Logging:LogLevel:Default", "Warning",
            "--Jwt:SigningKey", SigningKey
        ];
        if (accessTokenLifetime is { } lifetime)
            args.AddRange(["--Jwt:AccessTokenLifetime", lifetime.ToString("c")]);
        if (deviceLoginPermitsPerMinute is { } permits)
            args.AddRange(["--Auth:DeviceLoginPermitsPerMinute", permits.ToString()]);

        _app = ApiProgram.CreateApplication(args.ToArray(), roots, mapExtra);
    }

    public static async Task<AppHubTestHost> StartAsync(CancellationToken cancellationToken, TimeSpan? accessTokenLifetime = null,
        Action<WebApplication>? mapExtra = null, int? deviceLoginPermitsPerMinute = null)
    {
        var host = new AppHubTestHost(accessTokenLifetime, mapExtra, deviceLoginPermitsPerMinute);
        await host._app.StartAsync(cancellationToken);

        // kestrel puts the port it got for port 0 into the addresses
        var baseUrl = host._app.Urls.First();
        host.HubUrl = $"{baseUrl}/{Connections.HubName}";
        host.Http.BaseAddress = new Uri(baseUrl);
        return host;
    }

    // registers an account with its device like the seed, signs the device in with a code from the panel and
    // connects it; ready when every module knows the user, like a client that waits for the seed
    public async Task<HubTestClient> ConnectAsync(string login, CancellationToken cancellationToken)
    {
        var userId = await RegisterAsync(login);
        var tokens = await SignInDeviceAsync(login, userId);

        var client = new HubTestClient(HubUrl, Http, login, tokens);
        _clients.Add(client);
        await client.StartAsync(cancellationToken);

        await WaitForUserAsync(client.UserId);

        return client;
    }

    // the owner creates the group over the hub, members join with its code; ready when orchestration has them all
    public async Task<GroupCreatedNotification> CreateGroupAsync(HubTestClient owner, string groupName, params HubTestClient[] members)
    {
        await owner.Hub.CreateGroup(groupName);
        var group = await owner.Events.WaitForAsync<GroupCreatedNotification>(g => g.Name == groupName, $"group {groupName} created");

        foreach (var member in members)
        {
            await member.Hub.JoinGroup(group.GroupId, group.JoinCode);
            await member.Events.WaitForAsync<GroupSnapshot>(g => g.GroupId == group.GroupId, $"{member.Identifier} joined {groupName}");
        }

        foreach (var device in members.Prepend(owner))
            await ModulesHost.WaitUntilAsync(async () => await device.Hub.GetGroupAgentSettings(group.GroupId) is not null,
                $"{device.Identifier} in the orchestration group {groupName}");

        return group;
    }

    // the alias of a tool comes from the participant name, so a plan needs the name and the functions in orchestration
    public async Task WaitForParticipantAsync(Guid userId, string name, params string[] functionNames)
    {
        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(OrchestrationRoot,
            model => model.Query<Participant>().AnyAsync(p => p.Id.Match(userId) && p.Name == name)), $"participant {name}");

        await ModulesHost.WaitUntilAsync(async () =>
        {
            var functions = await GetParticipantFunctionNamesAsync(userId);
            return functionNames.All(functions.Contains);
        }, $"functions of {name}");
    }

    // names of the functions orchestration can plan with, sorted
    public async Task<IReadOnlyList<string>> GetParticipantFunctionNamesAsync(Guid userId)
    {
        var functions = await QueryAsync<IOrchestrationModule, List<ParticipantFunctionDto>>(module =>
            module.ExecuteQueryAsync<GetParticipantFunctionsQuery, List<ParticipantFunctionDto>>(new GetParticipantFunctionsQuery(userId)));

        return functions.Select(f => f.Name).Order().ToList();
    }

    public async Task<TResult> QueryAsync<TModule, TResult>(Func<TModule, Task<TResult>> query) where TModule : notnull
    {
        await using var scope = _app.Services.CreateAsyncScope();
        return await query(scope.ServiceProvider.GetRequiredService<TModule>());
    }

    // there is no registration endpoint: an account and one device with the id of the account, through the module
    // like the seed; the e-mail and the password of every test account follow from the login
    public async Task<Guid> RegisterAsync(string login)
    {
        var userId = await QueryAsync<IIdentityModule, Guid>(module => module.ExecuteCommandAsync<RegisterUserCommand, Guid>(
            new RegisterUserCommand(login, $"{login}@test.local", ModulesHost.TestPassword)));
        await RegisterDeviceAsync(userId, userId);
        return userId;
    }

    // a further device of the account, as the devices module will create it (TASK-ENG-029)
    public Task RegisterDeviceAsync(Guid userId, Guid deviceId)
    {
        return QueryAsync<IIdentityModule, bool>(async module =>
        {
            await module.ExecuteCommandAsync(new RegisterDeviceSessionCommand(deviceId, userId));
            return true;
        });
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
    public async Task<HttpResponseMessage> SendAsPanelAsync(HttpMethod method, string path, string? cookie)
    {
        using var request = new HttpRequestMessage(method, path);
        if (cookie != null)
            request.Headers.Add("Cookie", cookie);

        return await Http.SendAsync(request);
    }

    public Task<HttpResponseMessage> RequestLoginCodeAsync(string? cookie, Guid deviceId)
    {
        return SendAsPanelAsync(HttpMethod.Post, AuthRoutes.DeviceLoginCode(deviceId), cookie);
    }

    public Task<HttpResponseMessage> DeviceLoginAsync(string code)
    {
        return Http.PostAsJsonAsync(AuthRoutes.DeviceLogin, new DeviceLoginRequest(code));
    }

    // the whole sign-in of a device: the panel signs in, issues a code for the device and the device signs in with it
    public async Task<AuthTokensResponse> SignInDeviceAsync(string login, Guid deviceId)
    {
        var cookie = await SignInAccountAsync(login);

        var codeResponse = await RequestLoginCodeAsync(cookie, deviceId);
        await EnsureAsync(codeResponse, $"login code of {login}");
        var code = (await codeResponse.Content.ReadFromJsonAsync<DeviceLoginCodeResponse>())!;

        var response = await DeviceLoginAsync(code.Code);
        await EnsureAsync(response, $"device sign-in of {login}");
        return (await response.Content.ReadFromJsonAsync<AuthTokensResponse>())!;
    }

    private static async Task EnsureAsync(HttpResponseMessage response, string what)
    {
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"{what} failed: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
    }

    private async Task WaitForUserAsync(Guid userId)
    {
        await ModulesHost.WaitUntilAsync(async () =>
        {
            var profiles = await QueryAsync<IProfilesModule, List<ProfileDto>>(module =>
                module.ExecuteQueryAsync<GetProfilesQuery, List<ProfileDto>>(new GetProfilesQuery([userId])));
            return profiles.Count == 1;
        }, "profile of the user");

        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(GroupsRoot,
            model => model.Query<Member>().AnyAsync(m => m.Id.Match(userId))), "groups member of the user");
        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(PluginsRoot,
            model => model.Query<PluginOwner>().AnyAsync(o => o.Id.Match(userId))), "plugin owner of the user");
        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(OrchestrationRoot,
            model => model.Query<Participant>().AnyAsync(p => p.Id.Match(userId))), "participant of the user");
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
