using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Events;

namespace Dara.Server.Modules.Identity.Application.CreateClientIdentity;

public record NewClientIdentityCreatedNotification : DomainEventNotificationBase<NewClientIdentityCreatedDomainEvent>
{
    public NewClientIdentityCreatedNotification(Guid notificationId, NewClientIdentityCreatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}