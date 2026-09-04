using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

public record ExecutionPlanId(Guid Value) : BaseEntityId(Value);