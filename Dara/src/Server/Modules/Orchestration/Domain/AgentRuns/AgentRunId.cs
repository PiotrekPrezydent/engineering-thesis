using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

public record AgentRunId(Guid Value) : BaseEntityId(Value);