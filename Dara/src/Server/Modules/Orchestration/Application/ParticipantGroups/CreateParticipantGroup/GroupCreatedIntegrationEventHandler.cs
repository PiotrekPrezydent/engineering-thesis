using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;
using Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;

namespace Dara.Server.Modules.Orchestration.Application.ParticipantGroups.CreateParticipantGroup;

public class GroupCreatedIntegrationEventHandler : IIntegrationEventHandler<GroupCreatedIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public GroupCreatedIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(GroupCreatedIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(
            new CreateParticipantGroupCommand(integrationEvent.GroupId, integrationEvent.GroupOwnerId));
    }
}