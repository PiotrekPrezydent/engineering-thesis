using Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Shared.Contracts.StateSnapshots;
using IntegrationPlannedCallSnapshot = Dara.Server.Modules.Orchestration.Integration.PlannedCallSnapshot;

namespace Dara.Server.Apps.API.Extensions;

// module dtos and integration snapshots in the shape of the client contract
public static class ContractMappingExtensions
{
    public static PluginSnapshot ToSnapshot(this OwnerPluginDto plugin)
    {
        var functions = plugin.Functions
            .Select(f => new PluginFunctionSnapshot(f.FunctionId, f.IsEnabled, f.Description))
            .ToList();

        return new PluginSnapshot(plugin.PluginId, plugin.Name, plugin.Description, plugin.Version, plugin.Sha256, plugin.IsEnabled, functions);
    }

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