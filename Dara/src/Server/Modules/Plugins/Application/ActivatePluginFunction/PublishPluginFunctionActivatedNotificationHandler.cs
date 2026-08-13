using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Plugins.Application.ActivatePluginFunction;

public class PublishPluginFunctionActivatedNotificationHandler : IDomainEventNotificationHandler<PluginFunctionActivatedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishPluginFunctionActivatedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(PluginFunctionActivatedNotification notification)
    {
        await _eventBus.PublishAsync(new PluginFunctionActivatedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.PluginOwnerId,
            notification.DomainEvent.PluginId,
            notification.DomainEvent.PluginFunctionId
            ));
    }
}