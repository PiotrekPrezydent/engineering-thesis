using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Plugins.Application.PluginOwners.CreatePluginOwner;

// one handler of an event per module: the client becomes a plugin owner and gets the roles Plugins gives every new
// client (PluginsRoles.ForNewSubjects); a repeated grant changes nothing in Authorization
public class UserClientRegisteredIntegrationEventHandler : IIntegrationEventHandler<UserClientRegisteredIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;
    private readonly IEventBus _eventBus;

    public UserClientRegisteredIntegrationEventHandler(IInternalCommandScheduler commandScheduler, IEventBus eventBus)
    {
        _commandScheduler = commandScheduler;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(UserClientRegisteredIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new CreatePluginOwnerCommand(integrationEvent.ClientId));

        foreach (var role in PluginsRoles.ForNewSubjects)
            await _eventBus.PublishAsync(role.ToGrant(integrationEvent.ClientId, integrationEvent.OccurredOn));
    }
}
