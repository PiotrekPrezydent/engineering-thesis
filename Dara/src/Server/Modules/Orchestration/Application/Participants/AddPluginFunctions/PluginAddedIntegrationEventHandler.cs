using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.AddPluginFunctions;

public class PluginAddedIntegrationEventHandler : IIntegrationEventHandler<PluginAddedIntegrationEvent>
{
    private readonly ICommandExecutor _commandExecutor;

    public PluginAddedIntegrationEventHandler(ICommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(PluginAddedIntegrationEvent integrationEvent)
    {
        var functionDatas = integrationEvent.PluginSnapshot.Functions
            .Select(e=>
                new FunctionData(
                    e.FunctionId,
                    e.Name,
                    e.Description,
                    e.ReturnTypeName, 
                    e.Parameters
                        .Select(p=>
                            new FunctionParameterData(
                                p.Name,
                                p.Description,
                                p.TypeName))
                        .ToList()
                    )).ToList();
        
        await _commandExecutor.ExecuteAsync(new AddPluginFunctionsCommand(integrationEvent.PluginOwnerId, functionDatas));
    }
}