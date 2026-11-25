using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Orchestration.Integration;

public record AgentRunStartedIntegrationEvent : IntegrationEventBase
{
    public Guid AgentRunId { get; init; }
    public Guid GroupId { get; init; }
    public Guid AuthorId { get; init; }
    public Guid MessageId { get; init; }
    public AgentRunStartedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid agentRunId, Guid groupId, Guid authorId, Guid messageId) : base(eventId, occurredOn)
    {
        AgentRunId = agentRunId;
        GroupId = groupId;
        AuthorId = authorId;
        MessageId = messageId;
    }
}
