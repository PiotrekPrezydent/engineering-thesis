using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.PublishAvailableFunctions;

public record PluginDisabledNotification : DomainEventNotificationBase<PluginDisabledDomainEvent>
{
    public PluginDisabledNotification(Guid notificationId, PluginDisabledDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
