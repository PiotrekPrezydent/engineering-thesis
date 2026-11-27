using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Users.Events;

namespace Dara.Server.Modules.Identity.Application.RevokeClient;

public record UserClientRevokedNotification : DomainEventNotificationBase<UserClientRevokedDomainEvent>
{
    public UserClientRevokedNotification(Guid notificationId, UserClientRevokedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
