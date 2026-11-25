using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

public interface IAgentRunRepository : IRepository
{
    public Task AddAsync(AgentRun agentRun);

    public Task<AgentRun> GetByIdAsync(AgentRunId id);
}
