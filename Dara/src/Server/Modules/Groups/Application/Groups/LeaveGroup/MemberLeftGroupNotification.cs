using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Domain.Groups.Events;

namespace Dara.Server.Modules.Groups.Application.Groups.LeaveGroup;

public record MemberLeftGroupNotification : DomainEventNotificationBase<MemberLeftGroupDomainEvent>
{
    public MemberLeftGroupNotification(Guid notificationId, MemberLeftGroupDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}