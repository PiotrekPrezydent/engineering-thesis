using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.Plugins.Events;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.PublishAvailableFunctions;

public record PluginAddedNotification : DomainEventNotificationBase<PluginAddedDomainEvent>
{
    public PluginAddedNotification(Guid notificationId, PluginAddedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
