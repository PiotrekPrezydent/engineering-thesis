using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Events;

// the model is called after the run is saved, never inside the command that starts the run
public record PlanGenerationRequestedDomainEvent(AgentRunId AgentRunId) : DomainEventBase;
