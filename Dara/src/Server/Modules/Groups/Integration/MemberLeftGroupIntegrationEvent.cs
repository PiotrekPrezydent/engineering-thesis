using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record MemberLeftGroupIntegrationEvent : IntegrationEventBase
{
    public Guid GroupId { get; }
    public Guid MemberId { get; }
    public MemberLeftGroupIntegrationEvent(Guid eventId, DateTime occurredOn, Guid groupId, Guid memberId) : base(eventId, occurredOn)
    {
        GroupId = groupId;
        MemberId = memberId;
    }
}