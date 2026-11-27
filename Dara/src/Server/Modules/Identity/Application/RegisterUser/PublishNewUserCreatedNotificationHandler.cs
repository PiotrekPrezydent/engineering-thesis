using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration.Authorization;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.RegisterUser;

// the new account is published with its panel client, created together; the roles of the account panel go to the
// panel client, never to the account (a subject has only its own roles; roles of the user itself come later)
public class PublishNewUserCreatedNotificationHandler : IDomainEventNotificationHandler<PanelClientCreatedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishNewUserCreatedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(PanelClientCreatedNotification notification)
    {
        var domainEvent = notification.DomainEvent;
        await _eventBus.PublishAsync(new NewUserCreatedIntegrationEvent(notification.NotificationId, domainEvent.OccuredOn,
            domainEvent.UserId.Value, domainEvent.ClientId.Value));

        // the panel holds its account: Identity.Owner on the account (ScopeId = the user id)
        await _eventBus.PublishAsync(new GrantScopeRoleIntegrationEvent(Guid.NewGuid(), domainEvent.OccuredOn,
            domainEvent.ClientId.Value, IdentityRoles.Owner.Code, IdentityScopes.User.Code, domainEvent.UserId.Value));

        foreach (var role in IdentityRoles.ForNewPanel)
            await _eventBus.PublishAsync(role.ToGrant(domainEvent.ClientId.Value, domainEvent.OccuredOn));
    }
}
