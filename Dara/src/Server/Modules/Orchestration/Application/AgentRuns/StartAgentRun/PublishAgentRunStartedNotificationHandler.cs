using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Orchestration.Integration;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;

public class PublishAgentRunStartedNotificationHandler : IDomainEventNotificationHandler<AgentRunStartedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishAgentRunStartedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(AgentRunStartedNotification notification)
    {
        await _eventBus.PublishAsync(new AgentRunStartedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.AgentRunId.Value,
            notification.DomainEvent.GroupId.Value,
            notification.DomainEvent.AuthorId.Value,
            notification.DomainEvent.MessageId));
    }
}
