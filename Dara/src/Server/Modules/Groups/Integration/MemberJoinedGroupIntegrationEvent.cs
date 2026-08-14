using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record MemberJoinedGroupIntegrationEvent : IntegrationEventBase
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
    public MemberJoinedGroupIntegrationEvent(Guid eventId, DateTime occurredOn, Guid groupId, Guid memberId) : base(eventId, occurredOn)
    {
        GroupId = groupId;
        MemberId = memberId;
    }
}