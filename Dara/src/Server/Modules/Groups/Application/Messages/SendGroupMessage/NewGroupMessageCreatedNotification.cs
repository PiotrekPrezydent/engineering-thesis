using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Domain.GroupMessages.Events;

namespace Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;

public record NewGroupMessageCreatedNotification : DomainEventNotificationBase<NewGroupMessageCreatedDomainEvent>
{
    public NewGroupMessageCreatedNotification(Guid notificationId, NewGroupMessageCreatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}