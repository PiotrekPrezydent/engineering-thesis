using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Groups.RemoveParticipantFromGroup;

public class MemberLeftGroupIntegrationEventHandler : IIntegrationEventHandler<MemberLeftGroupIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public MemberLeftGroupIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(MemberLeftGroupIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new RemoveParticipantFromGroupCommand(integrationEvent.GroupId, integrationEvent.MemberId));
    }
}