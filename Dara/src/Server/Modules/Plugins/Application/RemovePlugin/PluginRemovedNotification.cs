using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Events;

namespace Dara.Server.Modules.Plugins.Application.RemovePlugin;

public record PluginRemovedNotification : DomainEventNotificationBase<PluginRemovedDomainEvent>
{
    public PluginRemovedNotification(Guid notificationId, PluginRemovedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
