using Dara.Shared.Contracts.Plugins;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Shared.Contracts;

public interface IAppHub
{
    public Task CreateGroup(string groupName);
    
    public Task JoinGroup(Guid groupId, string joinCode);
    
    public Task LeaveGroup(Guid groupId);
    
    public Task SendGroupMessage(Guid groupId, string message);
    
    // the full set of the client plugins after every connection, it replaces the previous set
    public Task RegisterPlugins(IReadOnlyList<PluginManifest> plugins);

    public Task<IReadOnlyList<PluginSnapshot>> GetMyPlugins();

    public Task EnablePlugin(Guid pluginId);

    public Task DisablePlugin(Guid pluginId);

    public Task EnablePluginFunction(Guid pluginId, Guid functionId);

    public Task DisablePluginFunction(Guid pluginId, Guid functionId);

    // null for anyone outside the group
    public Task<GroupAgentSettingsSnapshot?> GetGroupAgentSettings(Guid groupId);

    public Task EditGroupPromptMessagePrefix(Guid groupId, string promptMessagePrefix);

    public Task EditGroupChatClientProviderName(Guid groupId, string chatClientProviderName);

    public Task EditGroupClientModelName(Guid groupId, string clientModelName);

    public Task EditGroupApiKey(Guid groupId, string apiKey);

    // null for anyone but the author of the prompt
    public Task<AgentRunSnapshot?> GetAgentRun(Guid agentRunId);

    public Task ApproveExecutionPlan(Guid agentRunId);

    public Task RejectExecutionPlan(Guid agentRunId);
}

