using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.RejectExecutionPlan;

// the author decides about the whole plan at once
public class RejectExecutionPlanCommandHandler : ICommandHandler<RejectExecutionPlanCommand>
{
    private readonly IAgentRunRepository _repository;

    public RejectExecutionPlanCommandHandler(IAgentRunRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(RejectExecutionPlanCommand command)
    {
        var agentRun = await _repository.GetByIdAsync(new AgentRunId(command.AgentRunId));
        agentRun.RejectPlan(new ParticipantId(command.DeciderId));
    }
}
