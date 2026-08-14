using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.RemovePluginFunctions;

public class PluginRemovedIntegrationEventHandler : IIntegrationEventHandler<PluginRemovedIntegrationEvent>
{
    public async Task HandleAsync(PluginRemovedIntegrationEvent integrationEvent)
    {
        throw new NotImplementedException();
    }
}