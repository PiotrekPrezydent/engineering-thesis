using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Groups.CreateGroup;

public class NewGroupCreatedIntegrationEventHandler : IIntegrationEventHandler<NewGroupCreatedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public NewGroupCreatedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(NewGroupCreatedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(
            new CreateGroupCommand(integrationEvent.GroupId, integrationEvent.GroupOwnerId));
    }
}