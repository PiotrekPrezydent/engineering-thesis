using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns;

public class AgentRunRepository : IAgentRunRepository
{
    private readonly OrchestrationContext _context;

    public AgentRunRepository(OrchestrationContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AgentRun agentRun)
    {
        await _context.AgentRuns.AddAsync(agentRun);
    }

    public async Task<AgentRun> GetByIdAsync(AgentRunId id)
    {
        return await _context.AgentRuns.Include(x => x.Plan).FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("AgentRun", id);
    }
}
