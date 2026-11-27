using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.ChangeClientContext;

public class PublishUserClientContextChangedNotificationHandler : IDomainEventNotificationHandler<UserClientContextChangedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishUserClientContextChangedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(UserClientContextChangedNotification notification)
    {
        var domainEvent = notification.DomainEvent;
        await _eventBus.PublishAsync(new UserClientContextChangedIntegrationEvent(notification.NotificationId, domainEvent.OccuredOn,
            domainEvent.ClientId.Value, domainEvent.UserId.Value, domainEvent.Context.ToDictionary()));
    }
}
