using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Groups.AddParticipantToGroup;

public class NewMemberJoinedGroupIntegrationEventHandler : IIntegrationEventHandler<MemberJoinedGroupIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public NewMemberJoinedGroupIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(MemberJoinedGroupIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new AddParticipantToGroupCommand(integrationEvent.GroupId,integrationEvent.MemberId));
    }
}