using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.RejectExecutionPlan;

public record AgentRunRejectedNotification : DomainEventNotificationBase<AgentRunRejectedDomainEvent>
{
    public AgentRunRejectedNotification(Guid notificationId, AgentRunRejectedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
