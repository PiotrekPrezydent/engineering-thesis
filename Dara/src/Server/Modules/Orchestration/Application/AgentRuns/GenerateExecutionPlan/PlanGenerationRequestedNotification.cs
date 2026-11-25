using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

public record PlanGenerationRequestedNotification : DomainEventNotificationBase<PlanGenerationRequestedDomainEvent>
{
    public PlanGenerationRequestedNotification(Guid notificationId, PlanGenerationRequestedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
