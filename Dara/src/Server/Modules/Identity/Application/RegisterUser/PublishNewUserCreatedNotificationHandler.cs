using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.RegisterUser;

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

        // the roles Identity gives every new account (its own account); Authorization gets them without a reference to it
        foreach (var role in IdentityRoles.ForNewUser)
            await _eventBus.PublishAsync(role.ToGrant(notification.DomainEvent.UserId, notification.DomainEvent.OccuredOn));
    }
}
