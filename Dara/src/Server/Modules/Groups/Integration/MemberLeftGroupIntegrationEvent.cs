using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record MemberLeftGroupIntegrationEvent : IntegrationEventBase
{
    public Guid GroupId { get; }
    public Guid GroupMemberId { get; }
    public MemberLeftGroupIntegrationEvent(Guid eventId, DateTime occurredOn, Guid groupId, Guid groupMemberId) : base(eventId, occurredOn)
    {
        GroupId = groupId;
        GroupMemberId = groupMemberId;
    }
}