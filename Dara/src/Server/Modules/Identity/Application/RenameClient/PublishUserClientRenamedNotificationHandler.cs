using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.RenameClient;

public class PublishUserClientRenamedNotificationHandler : IDomainEventNotificationHandler<UserClientRenamedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishUserClientRenamedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(UserClientRenamedNotification notification)
    {
        var domainEvent = notification.DomainEvent;
        await _eventBus.PublishAsync(new UserClientRenamedIntegrationEvent(notification.NotificationId, domainEvent.OccuredOn,
            domainEvent.ClientId.Value, domainEvent.UserId.Value, domainEvent.Name));
    }
}
