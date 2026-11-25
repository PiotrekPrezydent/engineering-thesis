using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Orchestration.Integration;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

public class PublishPlanGenerationFailedNotificationHandler : IDomainEventNotificationHandler<PlanGenerationFailedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishPlanGenerationFailedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(PlanGenerationFailedNotification notification)
    {
        await _eventBus.PublishAsync(new ExecutionPlanGenerationFailedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.AgentRunId.Value,
            notification.DomainEvent.GroupId.Value,
            notification.DomainEvent.AuthorId.Value,
            notification.DomainEvent.Reason));
    }
}
