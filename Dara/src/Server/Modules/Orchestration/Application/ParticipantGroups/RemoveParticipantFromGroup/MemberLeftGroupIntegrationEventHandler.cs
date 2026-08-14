using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;
using Dara.Server.Modules.Orchestration.Application.Participants;

namespace Dara.Server.Modules.Orchestration.Application.ParticipantGroups.RemoveParticipantFromGroup;

public class MemberLeftGroupIntegrationEventHandler : IIntegrationEventHandler<MemberLeftGroupIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public MemberLeftGroupIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(MemberLeftGroupIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(new RemoveParticipantFromGroupCommand(integrationEvent.GroupId, integrationEvent.MemberId));
    }
}