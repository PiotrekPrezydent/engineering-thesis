using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration.Authorization;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.RegisterClient;

public class PublishUserClientRegisteredNotificationHandler : IDomainEventNotificationHandler<UserClientRegisteredNotification>
{
    private readonly IEventBus _eventBus;
    private readonly IClientRepository _clients;

    public PublishUserClientRegisteredNotificationHandler(IEventBus eventBus, IClientRepository clients)
    {
        _eventBus = eventBus;
        _clients = clients;
    }

    public async Task HandleAsync(UserClientRegisteredNotification notification)
    {
        var domainEvent = notification.DomainEvent;
        await _eventBus.PublishAsync(new UserClientRegisteredIntegrationEvent(notification.NotificationId, domainEvent.OccuredOn,
            domainEvent.ClientId.Value, domainEvent.UserId.Value, domainEvent.Name, domainEvent.Type.ToString()));

        // the roles Identity gives every new client (itself); a client has only its own roles, so the panel client of
        // its account, where it was added, gets full control of it as well
        foreach (var role in IdentityRoles.ForNewClient)
            await _eventBus.PublishAsync(role.ToGrant(domainEvent.ClientId.Value, domainEvent.OccuredOn));

        var panel = await _clients.GetPanelAsync(domainEvent.UserId);
        await _eventBus.PublishAsync(new GrantScopeRoleIntegrationEvent(Guid.NewGuid(), domainEvent.OccuredOn,
            panel.Id.Value, IdentityRoles.ClientOwner.Code, IdentityScopes.Client.Code, domainEvent.ClientId.Value));
    }
}
