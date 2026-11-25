using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

// a model that answers with text only has made no plan, the run then fails with the reason
public class ExecutionPlanMustHaveCallsRule : IBuisnessRule
{
    private readonly ExecutionPlan _plan;

    public ExecutionPlanMustHaveCallsRule(ExecutionPlan plan)
    {
        _plan = plan;
    }

    public string Message => "The execution plan has no calls";
    public bool IsBroken()
    {
        return _plan.Calls.Count == 0;
    }
}
