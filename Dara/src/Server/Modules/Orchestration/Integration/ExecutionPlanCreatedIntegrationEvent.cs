using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Orchestration.Integration;

// the plan waits for the decision of its author
public record ExecutionPlanCreatedIntegrationEvent : IntegrationEventBase
{
    public Guid AgentRunId { get; init; }
    public Guid GroupId { get; init; }
    public Guid AuthorId { get; init; }
    public IReadOnlyList<PlannedCallSnapshot> Calls { get; init; }
    public ExecutionPlanCreatedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid agentRunId, Guid groupId, Guid authorId, IReadOnlyList<PlannedCallSnapshot> calls) : base(eventId, occurredOn)
    {
        AgentRunId = agentRunId;
        GroupId = groupId;
        AuthorId = authorId;
        Calls = calls;
    }
}
