using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.CreateUser;

public class PublishNewUserCreatedNotificationHandler : IDomainEventNotificationHandler<NewUserCreatedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishNewUserCreatedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public async Task HandleAsync(NewUserCreatedNotification notification)
    {
        await _eventBus.PublishAsync(new NewUserCreatedIntegrationEvent(notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.UserId));
    }
}

