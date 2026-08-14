using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Domain.Groups.Events;

namespace Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;

public record NewGroupCreatedNotification : DomainEventNotificationBase<NewGroupCreatedDomainEvent>
{
    public NewGroupCreatedNotification(Guid notificationId, NewGroupCreatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}