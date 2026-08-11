using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record GroupCreatedIntegrationEvent : IntegrationEventBase
{
    public Guid GroupId { get; }
    public Guid GroupOwnerId { get; }
    
    public GroupCreatedIntegrationEvent(Guid id, DateTime occurredOn, Guid groupId, Guid groupOwnerId) : base(id, occurredOn)
    {
        GroupId = groupId;
        GroupOwnerId = groupOwnerId;
    }
}