using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record GroupMessageAddedIntegrationEvent : IntegrationEventBase
{
    public Guid MessageId { get; init; }
    public Guid GroupId { get; init; }
    public Guid AuthorId { get; init; }
    public string Content { get; init; }
    public GroupMessageAddedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid messageId, Guid groupId, Guid authorId, string content) : base(eventId, occurredOn)
    {
        MessageId = messageId;
        GroupId = groupId;
        AuthorId = authorId;
        Content = content;
    }
}