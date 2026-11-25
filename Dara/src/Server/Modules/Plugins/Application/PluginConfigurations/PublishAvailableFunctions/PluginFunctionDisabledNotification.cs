using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.PublishAvailableFunctions;

public record PluginFunctionDisabledNotification : DomainEventNotificationBase<PluginFunctionDisabledDomainEvent>
{
    public PluginFunctionDisabledNotification(Guid notificationId, PluginFunctionDisabledDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
