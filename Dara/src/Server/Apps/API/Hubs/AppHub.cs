using Dara.Server.Apps.API.Extensions;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;
using Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;
using Dara.Server.Modules.Groups.Application.Groups.LeaveGroup;
using Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.ApproveExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.RejectExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;
using Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditPromptMessagePrefix;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePluginFunction;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePluginFunction;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.ChangeProfileName;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Plugins;
using Dara.Shared.Contracts.StateSnapshots;
using Dara.Server.Apps.API.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

// a device token only (claims sub and device); an account token gets 403 at the negotiation
[Authorize(Policy = AuthPolicies.Device)]
public partial class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IIdentityModule _identityModule;
    private readonly IGroupsModule _groupsModule;
    private readonly IProfilesModule _profilesModule;
    private readonly IPluginsModule _pluginsModule;
    private readonly IOrchestrationModule _orchestrationModule;

    public AppHub(IIdentityModule identityModule, IProfilesModule profilesModule, IGroupsModule groupsModule, IPluginsModule pluginsModule, IOrchestrationModule orchestrationModule)
    {
        _identityModule = identityModule;
        _profilesModule = profilesModule;
        _groupsModule = groupsModule;
        _pluginsModule = pluginsModule;
        _orchestrationModule = orchestrationModule;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected - USER: {Context.GuidIdentifier()} DEVICE: {Context.DeviceId()}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        Console.WriteLine($"{Context.ConnectionId} DISCONNECTED - USER: {Context.GuidIdentifier()} DEVICE: {Context.DeviceId()}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task ChangeName(string name)
    {
        var command = new ChangeProfileNameCommand(Context.GuidIdentifier(), name);
        await _profilesModule.ExecuteCommandAsync<ChangeProfileNameCommand>(command);
    }

    public async Task CreateGroup(string groupName)
    {
        var command = new CreateNewGroupCommand(Context.GuidIdentifier(), groupName,"DEFAULT-CODE"+Random.Shared.Next());
        await _groupsModule.ExecuteCommandAsync<CreateNewGroupCommand,Guid>(command);
    }

    public async Task JoinGroup(Guid groupId, string joinCode)
    {
        await _groupsModule.ExecuteCommandAsync(new JoinToGroupCommand(groupId,Context.GuidIdentifier(),joinCode));
    }

    public async Task LeaveGroup(Guid groupId)
    {
        await _groupsModule.ExecuteCommandAsync(new LeaveGroupCommand(groupId, Context.GuidIdentifier()));
    }

    public async Task SendGroupMessage(Guid groupId, string message)
    {
        await _groupsModule.ExecuteCommandAsync(new SendGroupMessageCommand(groupId, Context.GuidIdentifier(), message));
    }

    public async Task RegisterPlugins(IReadOnlyList<PluginManifest> plugins)
    {
        await _pluginsModule.ExecuteCommandAsync(new RegisterPluginsCommand(Context.GuidIdentifier(), plugins));
    }

    public async Task<IReadOnlyList<PluginSnapshot>> GetMyPlugins()
    {
        var plugins = await _pluginsModule.ExecuteQueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(Context.GuidIdentifier()));
        return plugins.Select(p => p.ToSnapshot()).ToList();
    }

    public async Task EnablePlugin(Guid pluginId)
    {
        await _pluginsModule.ExecuteCommandAsync(new EnablePluginCommand(Context.GuidIdentifier(), pluginId));
    }

    public async Task DisablePlugin(Guid pluginId)
    {
        await _pluginsModule.ExecuteCommandAsync(new DisablePluginCommand(Context.GuidIdentifier(), pluginId));
    }

    public async Task EnablePluginFunction(Guid pluginId, Guid functionId)
    {
        await _pluginsModule.ExecuteCommandAsync(new EnablePluginFunctionCommand(Context.GuidIdentifier(), pluginId, functionId));
    }

    public async Task DisablePluginFunction(Guid pluginId, Guid functionId)
    {
        await _pluginsModule.ExecuteCommandAsync(new DisablePluginFunctionCommand(Context.GuidIdentifier(), pluginId, functionId));
    }

    public async Task<GroupAgentSettingsSnapshot?> GetGroupAgentSettings(Guid groupId)
    {
        var settings = await _orchestrationModule.ExecuteQueryAsync<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>(new GetGroupAgentSettingsQuery(groupId, Context.GuidIdentifier()));
        return settings?.ToSnapshot();
    }

    public async Task EditGroupPromptMessagePrefix(Guid groupId, string promptMessagePrefix)
    {
        await _orchestrationModule.ExecuteCommandAsync(new EditPromptMessagePrefixCommand(groupId, Context.GuidIdentifier(), promptMessagePrefix));
    }

    public async Task EditGroupChatClientProviderName(Guid groupId, string chatClientProviderName)
    {
        await _orchestrationModule.ExecuteCommandAsync(new EditChatClientProviderNameCommand(groupId, Context.GuidIdentifier(), chatClientProviderName));
    }

    public async Task EditGroupClientModelName(Guid groupId, string clientModelName)
    {
        await _orchestrationModule.ExecuteCommandAsync(new EditClientModelNameCommand(groupId, Context.GuidIdentifier(), clientModelName));
    }

    public async Task EditGroupApiKey(Guid groupId, string apiKey)
    {
        await _orchestrationModule.ExecuteCommandAsync(new EditApiKeyCommand(groupId, Context.GuidIdentifier(), apiKey));
    }

    public async Task<AgentRunSnapshot?> GetAgentRun(Guid agentRunId)
    {
        var agentRun = await _orchestrationModule.ExecuteQueryAsync<GetAgentRunQuery, AgentRunDto?>(new GetAgentRunQuery(agentRunId, Context.GuidIdentifier()));
        return agentRun?.ToSnapshot();
    }

    public async Task ApproveExecutionPlan(Guid agentRunId)
    {
        await _orchestrationModule.ExecuteCommandAsync(new ApproveExecutionPlanCommand(agentRunId, Context.GuidIdentifier()));
    }

    public async Task RejectExecutionPlan(Guid agentRunId)
    {
        await _orchestrationModule.ExecuteCommandAsync(new RejectExecutionPlanCommand(agentRunId, Context.GuidIdentifier()));
    }
}
