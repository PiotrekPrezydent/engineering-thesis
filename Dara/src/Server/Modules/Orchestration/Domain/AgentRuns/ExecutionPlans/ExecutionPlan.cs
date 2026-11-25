using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

// calls numbered 1..n in the order the model gave them, each uses results of earlier calls only
public class ExecutionPlan : Entity
{
    public ExecutionPlanId Id { get; private set; }
    
    public AgentRunId AgentRunId { get; private set; }
    
    public IReadOnlyList<PlannedCall> Calls => _calls.AsReadOnly();
    private List<PlannedCall> _calls;

    public int NextIndex => _calls.Count + 1;

    private ExecutionPlan() { }

    private ExecutionPlan(AgentRunId agentRunId)
    {
        Id = new ExecutionPlanId(Guid.NewGuid());
        AgentRunId = agentRunId;
        _calls = new List<PlannedCall>();
    }

    internal static ExecutionPlan CreateEmpty(AgentRunId agentRunId)
    {
        return new ExecutionPlan(agentRunId);
    }

    internal PlannedCall AddCall(ToolAlias tool, IReadOnlyList<CallArgument> arguments)
    {
        CheckRule(new ResultReferenceMustPointToEarlierCallRule(NextIndex, arguments));

        var call = new PlannedCall(NextIndex, tool.Alias, tool.ParticipantId, tool.FunctionId, arguments);
        _calls.Add(call);
        return call;
    }
}