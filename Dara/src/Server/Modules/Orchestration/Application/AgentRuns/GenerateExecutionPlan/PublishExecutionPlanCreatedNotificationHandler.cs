using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Integration;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// the outbox runs after the save, so the plan read here is the one the event is about
public class PublishExecutionPlanCreatedNotificationHandler : IDomainEventNotificationHandler<ExecutionPlanCreatedNotification>
{
    private readonly IEventBus _eventBus;
    private readonly IReadModel _readModel;

    public PublishExecutionPlanCreatedNotificationHandler(IEventBus eventBus, IReadModel readModel)
    {
        _eventBus = eventBus;
        _readModel = readModel;
    }

    public async Task HandleAsync(ExecutionPlanCreatedNotification notification)
    {
        var agentRun = await _readModel.Query<AgentRun>()
            .Include(r => r.Plan)
            .FirstAsync(r => r.Id.Match(notification.DomainEvent.AgentRunId.Value));

        await _eventBus.PublishAsync(new ExecutionPlanCreatedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.AgentRunId.Value,
            notification.DomainEvent.GroupId.Value,
            notification.DomainEvent.AuthorId.Value,
            agentRun.Plan.Calls.Select(c => PlannedCallMapper.ToSnapshot(agentRun, c)).ToList()));
    }
}
