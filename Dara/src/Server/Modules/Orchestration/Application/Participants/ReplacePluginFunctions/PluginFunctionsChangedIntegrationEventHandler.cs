using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.ReplacePluginFunctions;

public class PluginFunctionsChangedIntegrationEventHandler : IIntegrationEventHandler<PluginFunctionsChangedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public PluginFunctionsChangedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    // the plugin owner is the user, so the participant has the same id
    public async Task HandleAsync(PluginFunctionsChangedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new ReplacePluginFunctionsCommand(
            integrationEvent.PluginOwnerId,
            integrationEvent.PluginId,
            integrationEvent.Version,
            integrationEvent.Functions));
    }
}
