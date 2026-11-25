using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.PublishAvailableFunctions;

public record PluginConfigurationRemovedNotification : DomainEventNotificationBase<PluginConfigurationRemovedDomainEvent>
{
    public PluginConfigurationRemovedNotification(Guid notificationId, PluginConfigurationRemovedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
