using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

public record ExecutionPlanCreatedNotification : DomainEventNotificationBase<ExecutionPlanCreatedDomainEvent>
{
    public ExecutionPlanCreatedNotification(Guid notificationId, ExecutionPlanCreatedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
