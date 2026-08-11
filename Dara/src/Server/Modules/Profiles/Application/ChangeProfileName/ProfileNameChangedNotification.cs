using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Profiles.Domain.Events;

namespace Dara.Server.Modules.Profiles.Application.ChangeProfileName;

public record ProfileNameChangedNotification : DomainEventNotificationBase<ProfileNameChangedDomainEvent>
{
    public ProfileNameChangedNotification(Guid notificationId, ProfileNameChangedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}