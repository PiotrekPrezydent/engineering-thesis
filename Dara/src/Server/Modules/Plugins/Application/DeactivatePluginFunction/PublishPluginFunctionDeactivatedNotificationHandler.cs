using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Plugins.Application.DeactivatePluginFunction;

public class PublishPluginFunctionDeactivatedNotificationHandler : IDomainEventNotificationHandler<PluginFunctionDeactivatedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishPluginFunctionDeactivatedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(PluginFunctionDeactivatedNotification notification)
    {
        await _eventBus.PublishAsync(new PluginFunctionDeactivatedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.PluginOwnerId,
            notification.DomainEvent.PluginId,
            notification.DomainEvent.PluginFunctionId));
    }
}