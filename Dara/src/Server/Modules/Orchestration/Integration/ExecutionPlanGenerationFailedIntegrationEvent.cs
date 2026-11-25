using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Orchestration.Integration;

public record ExecutionPlanGenerationFailedIntegrationEvent : IntegrationEventBase
{
    public Guid AgentRunId { get; init; }
    public Guid GroupId { get; init; }
    public Guid AuthorId { get; init; }
    public string Reason { get; init; }
    public ExecutionPlanGenerationFailedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid agentRunId, Guid groupId, Guid authorId, string reason) : base(eventId, occurredOn)
    {
        AgentRunId = agentRunId;
        GroupId = groupId;
        AuthorId = authorId;
        Reason = reason;
    }
}
