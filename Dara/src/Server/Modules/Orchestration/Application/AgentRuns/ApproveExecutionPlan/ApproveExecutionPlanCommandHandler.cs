using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.ApproveExecutionPlan;

// the author decides about the whole plan at once
public class ApproveExecutionPlanCommandHandler : ICommandHandler<ApproveExecutionPlanCommand>
{
    private readonly IAgentRunRepository _repository;

    public ApproveExecutionPlanCommandHandler(IAgentRunRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(ApproveExecutionPlanCommand command)
    {
        var agentRun = await _repository.GetByIdAsync(new AgentRunId(command.AgentRunId));
        agentRun.ApprovePlan(new ParticipantId(command.DeciderId));
    }
}
