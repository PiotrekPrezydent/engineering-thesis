using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// runs in the outbox loop after the run is saved, so the command that starts the run never waits for the model
public class PlanGenerationRequestedNotificationHandler : IDomainEventNotificationHandler<PlanGenerationRequestedNotification>
{
    private readonly IInternalCommandScheduler _commandScheduler;

    public PlanGenerationRequestedNotificationHandler(IInternalCommandScheduler commandScheduler)
    {
        _commandScheduler = commandScheduler;
    }

    public async Task HandleAsync(PlanGenerationRequestedNotification notification)
    {
        await _commandScheduler.EnqueueAsync(new GenerateExecutionPlanCommand(notification.DomainEvent.AgentRunId.Value));
    }
}
