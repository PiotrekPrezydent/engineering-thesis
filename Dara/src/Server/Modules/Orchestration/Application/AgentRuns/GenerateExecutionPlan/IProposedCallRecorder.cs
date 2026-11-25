namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// takes the calls the model proposes while the plan is generated, one at a time in the order of the answer
public interface IProposedCallRecorder
{
    ProposedCallResult Record(string alias, string argumentsJson);
}
