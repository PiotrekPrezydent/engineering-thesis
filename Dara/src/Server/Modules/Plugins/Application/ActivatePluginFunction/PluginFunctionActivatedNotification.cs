using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins.Events;

namespace Dara.Server.Modules.Plugins.Application.ActivatePluginFunction;

public record PluginFunctionActivatedNotification : DomainEventNotificationBase<PluginFunctionActivatedDomainEvent>
{
    public PluginFunctionActivatedNotification(Guid notificationId, PluginFunctionActivatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}