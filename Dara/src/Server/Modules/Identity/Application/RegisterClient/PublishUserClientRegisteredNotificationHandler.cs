using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration.Authorization;
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

        // the roles Identity gives every new client (itself); a client has only its own roles, so its user, who added
        // it, gets full control of it as well
        foreach (var role in IdentityRoles.ForNewClient)
            await _eventBus.PublishAsync(role.ToGrant(domainEvent.ClientId.Value, domainEvent.OccuredOn));

        await _eventBus.PublishAsync(new GrantScopeRoleIntegrationEvent(Guid.NewGuid(), domainEvent.OccuredOn,
            domainEvent.UserId.Value, IdentityRoles.ClientOwner.Code, IdentityScopes.Client.Code, domainEvent.ClientId.Value));
    }
}
