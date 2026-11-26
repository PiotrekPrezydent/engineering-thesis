using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

public record GenerateExecutionPlanCommand(Guid AgentRunId) : IOrderedInternalCommand
{
    // queued commands of one run run in order
    [JsonIgnore]
    public string OrderingKey => $"run:{AgentRunId}";
}
