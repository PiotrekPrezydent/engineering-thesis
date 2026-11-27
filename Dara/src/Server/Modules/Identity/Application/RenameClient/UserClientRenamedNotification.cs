using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Users.Events;

namespace Dara.Server.Modules.Identity.Application.RenameClient;

public record UserClientRenamedNotification : DomainEventNotificationBase<UserClientRenamedDomainEvent>
{
    public UserClientRenamedNotification(Guid notificationId, UserClientRenamedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
