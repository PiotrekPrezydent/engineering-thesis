using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Groups.CreateGroup;

public class NewGroupCreatedIntegrationEventHandler : IIntegrationEventHandler<NewGroupCreatedIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public NewGroupCreatedIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(NewGroupCreatedIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(
            new CreateGroupCommand(integrationEvent.GroupId, integrationEvent.GroupOwnerId));
    }
}