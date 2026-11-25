using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Orchestration.Integration;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.RejectExecutionPlan;

public class PublishAgentRunRejectedNotificationHandler : IDomainEventNotificationHandler<AgentRunRejectedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishAgentRunRejectedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(AgentRunRejectedNotification notification)
    {
        await _eventBus.PublishAsync(new AgentRunRejectedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.AgentRunId.Value,
            notification.DomainEvent.GroupId.Value,
            notification.DomainEvent.AuthorId.Value));
    }
}
