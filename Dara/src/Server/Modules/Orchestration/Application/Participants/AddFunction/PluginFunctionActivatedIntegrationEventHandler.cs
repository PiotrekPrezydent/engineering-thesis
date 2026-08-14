using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.AddFunction;

public class PluginFunctionActivatedIntegrationEventHandler : IIntegrationEventHandler<PluginFunctionActivatedIntegrationEvent>
{
    private readonly ICommandExecutor _commandExecutor;

    public PluginFunctionActivatedIntegrationEventHandler(ICommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(PluginFunctionActivatedIntegrationEvent integrationEvent)
    {
    }
}