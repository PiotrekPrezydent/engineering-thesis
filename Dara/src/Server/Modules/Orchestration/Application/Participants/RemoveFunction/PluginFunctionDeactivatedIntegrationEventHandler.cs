using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.RemoveFunction;

public class PluginFunctionDeactivatedIntegrationEventHandler : IIntegrationEventHandler<PluginFunctionDeactivatedIntegrationEvent>
{
    public async Task HandleAsync(PluginFunctionDeactivatedIntegrationEvent integrationEvent)
    {
        throw new NotImplementedException();
    }
}