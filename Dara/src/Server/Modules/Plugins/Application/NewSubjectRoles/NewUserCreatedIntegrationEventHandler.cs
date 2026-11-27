using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Plugins.Application.NewSubjectRoles;

// the roles Plugins gives every new account (PluginsRoles.ForNewSubjects); a repeated grant changes nothing in Authorization
public class NewUserCreatedIntegrationEventHandler : IIntegrationEventHandler<NewUserCreatedIntegrationEvent>
{
    private readonly IEventBus _eventBus;

    public NewUserCreatedIntegrationEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(NewUserCreatedIntegrationEvent integrationEvent)
    {
        foreach (var role in PluginsRoles.ForNewSubjects)
            await _eventBus.PublishAsync(role.ToGrant(integrationEvent.CreatedUserId, integrationEvent.OccurredOn));
    }
}
