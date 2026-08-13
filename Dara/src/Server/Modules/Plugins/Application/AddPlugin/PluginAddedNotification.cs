using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Events;

namespace Dara.Server.Modules.Plugins.Application.AddPlugin;

public record PluginAddedNotification : DomainEventNotificationBase<PluginAddedDomainEvent>
{
    public PluginAddedNotification(Guid notificationId, PluginAddedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}