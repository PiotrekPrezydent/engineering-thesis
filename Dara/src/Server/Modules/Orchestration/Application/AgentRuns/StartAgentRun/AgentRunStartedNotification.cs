using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;

public record AgentRunStartedNotification : DomainEventNotificationBase<AgentRunStartedDomainEvent>
{
    public AgentRunStartedNotification(Guid notificationId, AgentRunStartedDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}
