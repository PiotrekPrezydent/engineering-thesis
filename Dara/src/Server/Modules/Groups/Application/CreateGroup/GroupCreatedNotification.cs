using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Domain.Groups.Events;

namespace Dara.Server.Modules.Groups.Application.CreateGroup;

public record GroupCreatedNotification : DomainEventNotificationBase<GroupCreatedDomainEvent>
{
    public GroupCreatedNotification(Guid notificationId, GroupCreatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}