using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration.Authorization;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.RevokeClient;

public class PublishUserClientRevokedNotificationHandler : IDomainEventNotificationHandler<UserClientRevokedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishUserClientRevokedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(UserClientRevokedNotification notification)
    {
        var domainEvent = notification.DomainEvent;
        await _eventBus.PublishAsync(new UserClientRevokedIntegrationEvent(notification.NotificationId, domainEvent.OccuredOn,
            domainEvent.ClientId.Value, domainEvent.UserId.Value));

        // a revoked client is gone for good: its assignments on itself and as a subject disappear
        await _eventBus.PublishAsync(new ScopeRemovedIntegrationEvent(Guid.NewGuid(), domainEvent.OccuredOn,
            IdentityScopes.Client.Code, domainEvent.ClientId.Value));
    }
}
