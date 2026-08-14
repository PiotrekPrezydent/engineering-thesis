using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Domain.GroupMessages.Events;

namespace Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;

public record GroupMessageAddedNotification : DomainEventNotificationBase<GroupMessageAddedDomainEvent>
{
    public GroupMessageAddedNotification(Guid notificationId, GroupMessageAddedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}