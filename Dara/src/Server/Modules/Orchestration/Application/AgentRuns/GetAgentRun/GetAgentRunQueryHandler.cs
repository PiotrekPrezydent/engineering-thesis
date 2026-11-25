using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;

public class GetAgentRunQueryHandler : IQueryHandler<GetAgentRunQuery, AgentRunDto?>
{
    private readonly IReadModel _readModel;

    public GetAgentRunQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<AgentRunDto?> HandleAsync(GetAgentRunQuery query)
    {
        var agentRun = await _readModel.Query<AgentRun>()
            .Include(r => r.Plan)
            .FirstOrDefaultAsync(r => r.Id.Match(query.AgentRunId) && r.AuthorId.Match(query.RequesterId));

        if (agentRun is null)
            return null;

        return new AgentRunDto(
            agentRun.Id.Value,
            agentRun.GroupId.Value,
            agentRun.AuthorId.Value,
            agentRun.MessageId,
            agentRun.Prompt,
            agentRun.Status.Value,
            agentRun.FailureReason,
            agentRun.Plan.Calls.Select(c => PlannedCallMapper.ToDto(agentRun, c)).ToList());
    }
}
