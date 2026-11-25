using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// the run checks every proposed call, a broken rule goes back to the model as an error instead of ending the loop
public class AgentRunCallRecorder : IProposedCallRecorder
{
    private readonly AgentRun _agentRun;

    public AgentRunCallRecorder(AgentRun agentRun)
    {
        _agentRun = agentRun;
    }

    public ProposedCallResult Record(string alias, string argumentsJson)
    {
        try
        {
            var index = _agentRun.ProposeCall(new ProposedCall(alias, argumentsJson));
            return new ProposedCallResult(index, null);
        }
        catch (BuisnessRuleValidationException exception)
        {
            return new ProposedCallResult(null, exception.Message);
        }
    }
}
