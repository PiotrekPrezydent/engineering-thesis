using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Orchestration.Application.ParticipantGroups.AddParticipantToGroup;

public class NewMemberJoinedGroupIntegrationEventHandler : IIntegrationEventHandler<NewMemberJoinedGroupIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public NewMemberJoinedGroupIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(NewMemberJoinedGroupIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(new AddParticipantToGroupCommand(integrationEvent.GroupId,integrationEvent.MemberId));
    }
}