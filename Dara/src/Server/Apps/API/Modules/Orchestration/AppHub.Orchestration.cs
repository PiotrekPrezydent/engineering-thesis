using Dara.Server.Apps.API.Modules.Orchestration;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.ApproveExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.RejectExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;
using Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditPromptMessagePrefix;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Shared.Contracts.StateSnapshots;

// the namespace of the hub, not of this folder: a partial class lives in one namespace
namespace Dara.Server.Apps.API.Common.Hubs;

public partial class AppHub
{
    public async Task<GroupAgentSettingsSnapshot?> GetGroupAgentSettings(Guid groupId)
    {
        var settings = await _orchestrationModule.ExecuteQueryAsync<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>(new GetGroupAgentSettingsQuery(groupId, Context.ClientId()));
        return settings?.ToSnapshot();
    }

    public async Task EditGroupPromptMessagePrefix(Guid groupId, string promptMessagePrefix)
    {
        await _orchestrationModule.ExecuteCommandAsync(new EditPromptMessagePrefixCommand(groupId, Context.ClientId(), promptMessagePrefix));
    }

    public async Task EditGroupChatClientProviderName(Guid groupId, string chatClientProviderName)
    {
        await _orchestrationModule.ExecuteCommandAsync(new EditChatClientProviderNameCommand(groupId, Context.ClientId(), chatClientProviderName));
    }

    public async Task EditGroupClientModelName(Guid groupId, string clientModelName)
    {
        await _orchestrationModule.ExecuteCommandAsync(new EditClientModelNameCommand(groupId, Context.ClientId(), clientModelName));
    }

    public async Task EditGroupApiKey(Guid groupId, string apiKey)
    {
        await _orchestrationModule.ExecuteCommandAsync(new EditApiKeyCommand(groupId, Context.ClientId(), apiKey));
    }

    public async Task<AgentRunSnapshot?> GetAgentRun(Guid agentRunId)
    {
        var agentRun = await _orchestrationModule.ExecuteQueryAsync<GetAgentRunQuery, AgentRunDto?>(new GetAgentRunQuery(agentRunId, Context.ClientId()));
        return agentRun?.ToSnapshot();
    }

    public async Task ApproveExecutionPlan(Guid agentRunId)
    {
        await _orchestrationModule.ExecuteCommandAsync(new ApproveExecutionPlanCommand(agentRunId, Context.ClientId()));
    }

    public async Task RejectExecutionPlan(Guid agentRunId)
    {
        await _orchestrationModule.ExecuteCommandAsync(new RejectExecutionPlanCommand(agentRunId, Context.ClientId()));
    }
}
