using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;

// every group message comes here, the group decides by its prefix whether it is a prompt
public class GroupMessageAddedIntegrationEventHandler : IIntegrationEventHandler<GroupMessageAddedIntegrationEvent>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public GroupMessageAddedIntegrationEventHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(GroupMessageAddedIntegrationEvent integrationEvent)
    {
        await _commandExecutor.ExecuteAsync(new StartAgentRunCommand(
            integrationEvent.MessageId,
            integrationEvent.GroupId,
            integrationEvent.AuthorId,
            integrationEvent.Content));
    }
}
