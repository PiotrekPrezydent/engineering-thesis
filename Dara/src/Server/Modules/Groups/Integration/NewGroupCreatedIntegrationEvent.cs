using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record NewGroupCreatedIntegrationEvent : IntegrationEventBase
{
    public Guid GroupId { get; }
    public Guid GroupOwnerId { get; }
    
    public NewGroupCreatedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid groupId, Guid groupOwnerId) : base(eventId, occurredOn)
    {
        GroupId = groupId;
        GroupOwnerId = groupOwnerId;
    }
}