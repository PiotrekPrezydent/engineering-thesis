using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;

// the plan belongs to its author, null for anyone else and for a run that does not exist
public record GetAgentRunQuery(Guid AgentRunId, Guid RequesterId) : IQuery<AgentRunDto?>;
