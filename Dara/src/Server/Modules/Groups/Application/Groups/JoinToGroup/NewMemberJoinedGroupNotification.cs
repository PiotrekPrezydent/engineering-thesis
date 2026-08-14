using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Domain.Groups.Events;

namespace Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;

public record NewMemberJoinedGroupNotification : DomainEventNotificationBase<MemberJoinedGroupDomainEvent>
{
    public NewMemberJoinedGroupNotification(Guid notificationId, MemberJoinedGroupDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}