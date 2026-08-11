using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record NewMemberJoinedGroupIntegrationEvent : IntegrationEventBase
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
    public NewMemberJoinedGroupIntegrationEvent(Guid eventId, DateTime occurredOn, Guid groupId, Guid memberId) : base(eventId, occurredOn)
    {
        GroupId = groupId;
        MemberId = memberId;
    }
}