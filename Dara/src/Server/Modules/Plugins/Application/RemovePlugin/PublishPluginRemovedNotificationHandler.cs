using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Plugins.Application.RemovePlugin;

public class PublishPluginRemovedNotificationHandler : IDomainEventNotificationHandler<PluginRemovedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishPluginRemovedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(PluginRemovedNotification notification)
    {
        await _eventBus.PublishAsync(new PluginRemovedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.PluginOwnerId,
            notification.DomainEvent.PluginId
            ));
    }
}