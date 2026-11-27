using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.RegisterClient;

public class PublishUserClientRegisteredNotificationHandler : IDomainEventNotificationHandler<UserClientRegisteredNotification>
{
    private readonly IEventBus _eventBus;

    public PublishUserClientRegisteredNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(UserClientRegisteredNotification notification)
    {
        var domainEvent = notification.DomainEvent;
        await _eventBus.PublishAsync(new UserClientRegisteredIntegrationEvent(notification.NotificationId, domainEvent.OccuredOn,
            domainEvent.ClientId.Value, domainEvent.UserId.Value, domainEvent.Name, domainEvent.Type.ToString()));
    }
}
