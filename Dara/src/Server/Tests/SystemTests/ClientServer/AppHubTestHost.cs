using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.Modules.Groups.Domain.Members;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.GetUser;
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

    public IdentityCompositionRoot IdentityRoot { get; } = new();
    public ProfilesCompositionRoot ProfilesRoot { get; } = new();
    public GroupsCompositionRoot GroupsRoot { get; } = new();
    public PluginsCompositionRoot PluginsRoot { get; } = new();
    public OrchestrationCompositionRoot OrchestrationRoot { get; }

    // no test calls the api of a model provider, every plan comes from this script
    public ScriptedChatClientProvider Model { get; } = new();

    public string HubUrl { get; private set; } = "";

    private AppHubTestHost()
    {
        OrchestrationRoot = new OrchestrationCompositionRoot([Model], new ExecutionPlanGeneratorOptions(TimeoutSeconds: 5));

        // same order as in the api host, the seed is off so tests see only their own data
        IModuleCompositionRoot[] roots = [IdentityRoot, GroupsRoot, ProfilesRoot, PluginsRoot, OrchestrationRoot];
        _app = ApiProgram.CreateApplication(
            ["--urls", "http://127.0.0.1:0", "--Seed:Enabled", "false", "--Logging:LogLevel:Default", "Warning"],
            roots);
    }

    public static async Task<AppHubTestHost> StartAsync(CancellationToken cancellationToken)
    {
        var host = new AppHubTestHost();
        await host._app.StartAsync(cancellationToken);

        // kestrel puts the port it got for port 0 into the addresses
        host.HubUrl = $"{host._app.Urls.First()}/{Connections.HubName}";
        return host;
    }

    // connects a device and waits until every module knows its user, like a client that waits for the seed
    public async Task<HubTestClient> ConnectAsync(string identifier, CancellationToken cancellationToken)
    {
        var client = new HubTestClient(HubUrl, identifier);
        _clients.Add(client);
        await client.StartAsync(cancellationToken);

        client.UserId = await GetUserIdAsync(identifier);
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

    private async Task<Guid> GetUserIdAsync(string identifier)
    {
        var userId = await QueryAsync<IIdentityModule, Guid?>(module =>
            module.ExecuteQueryAsync<GetUserQuery, Guid?>(new GetUserQuery(identifier)));

        return userId ?? throw new InvalidOperationException($"the connection of {identifier} created no user");
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
    }
}
