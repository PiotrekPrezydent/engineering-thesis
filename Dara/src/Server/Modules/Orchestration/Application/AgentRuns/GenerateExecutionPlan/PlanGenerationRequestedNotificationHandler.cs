using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// runs in the outbox loop after the run is saved, so the command that starts the run never waits for the model
public class PlanGenerationRequestedNotificationHandler : IDomainEventNotificationHandler<PlanGenerationRequestedNotification>
{
    private readonly IInternalCommandExecutor _commandExecutor;

    public PlanGenerationRequestedNotificationHandler(IInternalCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(PlanGenerationRequestedNotification notification)
    {
        await _commandExecutor.ExecuteAsync(new GenerateExecutionPlanCommand(notification.DomainEvent.AgentRunId.Value));
    }
}
