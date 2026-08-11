using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Events;

namespace Dara.Server.Modules.Identity.Application.CreateUser;

public record NewUserCreatedNotification : DomainEventNotificationBase<NewUserCreatedDomainEvent>
{
    public NewUserCreatedNotification(Guid notificationId, NewUserCreatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}