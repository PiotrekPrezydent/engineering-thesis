using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Orchestration.Integration;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.ApproveExecutionPlan;

public class PublishExecutionPlanApprovedNotificationHandler : IDomainEventNotificationHandler<ExecutionPlanApprovedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishExecutionPlanApprovedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(ExecutionPlanApprovedNotification notification)
    {
        await _eventBus.PublishAsync(new ExecutionPlanApprovedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.AgentRunId.Value,
            notification.DomainEvent.GroupId.Value,
            notification.DomainEvent.AuthorId.Value));
    }
}
