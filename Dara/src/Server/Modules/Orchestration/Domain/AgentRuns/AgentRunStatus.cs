using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

public record AgentRunStatus(string Value) : IValueObject
{
    internal static AgentRunStatus Starting => new(nameof(Starting));
    internal static AgentRunStatus Running => new AgentRunStatus(nameof(Running));
    internal static AgentRunStatus Completed => new AgentRunStatus(nameof(Completed));
}
