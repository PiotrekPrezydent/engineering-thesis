using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;

// every group message comes here, the group decides by its prefix whether it is a prompt
public class GroupMessageAddedIntegrationEventHandler : IIntegrationEventHandler<GroupMessageAddedIntegrationEvent>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public GroupMessageAddedIntegrationEventHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(GroupMessageAddedIntegrationEvent integrationEvent)
    {
        await _commandScheduler.EnqueueAsync(new StartAgentRunCommand(
            integrationEvent.MessageId,
            integrationEvent.GroupId,
            integrationEvent.AuthorId,
            integrationEvent.Content));
    }
}
