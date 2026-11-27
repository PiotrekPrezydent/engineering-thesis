using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.ChangeClientContext;

public record UserClientContextChangedNotification : DomainEventNotificationBase<UserClientContextChangedDomainEvent>
{
    public UserClientContextChangedNotification(Guid notificationId, UserClientContextChangedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
