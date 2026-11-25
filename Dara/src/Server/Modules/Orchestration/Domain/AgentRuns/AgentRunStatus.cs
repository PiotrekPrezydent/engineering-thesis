using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

// the run ends at the decision of the author, execution of the plan comes later
public record AgentRunStatus(string Value) : IValueObject
{
    internal static AgentRunStatus PlanGenerating => new(nameof(PlanGenerating));
    internal static AgentRunStatus PlanGenerationFailed => new(nameof(PlanGenerationFailed));
    internal static AgentRunStatus AwaitingApproval => new(nameof(AwaitingApproval));
    internal static AgentRunStatus Approved => new(nameof(Approved));
    internal static AgentRunStatus Rejected => new(nameof(Rejected));
}
