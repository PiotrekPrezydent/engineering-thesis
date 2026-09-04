using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;

public record NewAgentRunCreatedDomainEvent(AgentRunId Id) : DomainEventBase;