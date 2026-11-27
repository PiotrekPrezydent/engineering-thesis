using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Users.Events;

namespace Dara.Server.Modules.Identity.Application.RegisterClient;

public record UserClientRegisteredNotification : DomainEventNotificationBase<UserClientRegisteredDomainEvent>
{
    public UserClientRegisteredNotification(Guid notificationId, UserClientRegisteredDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
