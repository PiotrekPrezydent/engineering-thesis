using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.PublishAvailableFunctions;

public record PluginFunctionEnabledNotification : DomainEventNotificationBase<PluginFunctionEnabledDomainEvent>
{
    public PluginFunctionEnabledNotification(Guid notificationId, PluginFunctionEnabledDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
