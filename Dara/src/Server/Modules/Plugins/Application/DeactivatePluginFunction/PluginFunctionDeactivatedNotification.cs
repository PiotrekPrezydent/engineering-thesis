using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins.Events;

namespace Dara.Server.Modules.Plugins.Application.DeactivatePluginFunction;

public record PluginFunctionDeactivatedNotification : DomainEventNotificationBase<PluginFunctionDeactivatedDomainEvent>
{
    public PluginFunctionDeactivatedNotification(Guid notificationId, PluginFunctionDeactivatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}