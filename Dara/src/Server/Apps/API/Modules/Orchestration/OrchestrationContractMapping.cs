using Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Shared.Contracts.StateSnapshots;
using IntegrationPlannedCallSnapshot = Dara.Server.Modules.Orchestration.Integration.PlannedCallSnapshot;

namespace Dara.Server.Apps.API.Modules.Orchestration;

// dtos and integration snapshots of the orchestration module in the shape of the client contract
public static class OrchestrationContractMapping
{
    public static PlannedCallSnapshot ToSnapshot(this IntegrationPlannedCallSnapshot call)
    {
        return new PlannedCallSnapshot(call.Index, call.ParticipantId, call.ParticipantName, call.FunctionId, call.FunctionName, call.ArgumentsJson);
    }

    public static PlannedCallSnapshot ToSnapshot(this PlannedCallDto call)
    {
        return new PlannedCallSnapshot(call.Index, call.ParticipantId, call.ParticipantName, call.FunctionId, call.FunctionName, call.ArgumentsJson);
    }

    public static AgentRunSnapshot ToSnapshot(this AgentRunDto agentRun)
    {
        return new AgentRunSnapshot(agentRun.AgentRunId, agentRun.GroupId, agentRun.AuthorId, agentRun.MessageId, agentRun.Prompt,
            agentRun.Status, agentRun.FailureReason, agentRun.Calls.Select(c => c.ToSnapshot()).ToList());
    }

    public static GroupAgentSettingsSnapshot ToSnapshot(this GroupAgentSettingsDto settings)
    {
        return new GroupAgentSettingsSnapshot(settings.GroupId, settings.OwnerId, settings.PromptMessagePrefix,
            settings.ChatClientProviderName, settings.ClientModelName, settings.ApiKey);
    }
}
