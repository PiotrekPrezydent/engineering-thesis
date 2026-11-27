using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Clients.Events;

namespace Dara.Server.Modules.Identity.Application.RegisterUser;

public record PanelClientCreatedNotification : DomainEventNotificationBase<PanelClientCreatedDomainEvent>
{
    public PanelClientCreatedNotification(Guid notificationId, PanelClientCreatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
