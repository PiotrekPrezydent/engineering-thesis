using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Orchestration.Integration;

public record AgentRunRejectedIntegrationEvent : IntegrationEventBase
{
    public Guid AgentRunId { get; init; }
    public Guid GroupId { get; init; }
    public Guid AuthorId { get; init; }
    public AgentRunRejectedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid agentRunId, Guid groupId, Guid authorId) : base(eventId, occurredOn)
    {
        AgentRunId = agentRunId;
        GroupId = groupId;
        AuthorId = authorId;
    }
}
