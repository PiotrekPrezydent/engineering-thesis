using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.ApproveExecutionPlan;

public record ExecutionPlanApprovedNotification : DomainEventNotificationBase<ExecutionPlanApprovedDomainEvent>
{
    public ExecutionPlanApprovedNotification(Guid notificationId, ExecutionPlanApprovedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
