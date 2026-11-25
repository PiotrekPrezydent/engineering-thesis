using System.Text.Json.Nodes;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;
using Dara.Server.Modules.Orchestration.Integration;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns;

// a planned call by ids shown with the names the run saw and the arguments as the model wrote them
public static class PlannedCallMapper
{
    public static PlannedCallSnapshot ToSnapshot(AgentRun agentRun, PlannedCall call)
    {
        return new PlannedCallSnapshot(
            call.Index,
            call.ParticipantId.Value,
            ParticipantName(agentRun, call),
            call.FunctionId.Value,
            FunctionName(agentRun, call),
            ArgumentsJson(call));
    }

    public static PlannedCallDto ToDto(AgentRun agentRun, PlannedCall call)
    {
        return new PlannedCallDto(
            call.Index,
            call.Alias,
            call.ParticipantId.Value,
            ParticipantName(agentRun, call),
            call.FunctionId.Value,
            FunctionName(agentRun, call),
            ArgumentsJson(call));
    }

    private static string ParticipantName(AgentRun agentRun, PlannedCall call)
    {
        return agentRun.ParticipantsSnapshots.First(p => p.ParticipantId == call.ParticipantId.Value).Name;
    }

    private static string FunctionName(AgentRun agentRun, PlannedCall call)
    {
        return agentRun.FindFunction(new ToolAlias(call.Alias, call.ParticipantId, call.FunctionId))!.Description.Name;
    }

    private static string ArgumentsJson(PlannedCall call)
    {
        var arguments = new JsonObject();
        foreach (var argument in call.Arguments)
            arguments[argument.Name] = JsonNode.Parse(argument.Json);

        return arguments.ToJsonString();
    }
}
