using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantGroups;

public class MemberLeftGroupIntegrationEventHandler : IIntegrationEventHandler<MemberLeftGroupIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public MemberLeftGroupIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(MemberLeftGroupIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(new UpdateParticipantGroupCommand(integrationEvent.MemberId, integrationEvent.GroupId, UpdateProjectionListOption.Remove));
    }
}