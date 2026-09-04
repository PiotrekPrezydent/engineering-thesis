using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

public class ExecutionPlan : Entity
{
    public ExecutionPlanId Id { get; private set; }
    
    public AgentRun AgentRun { get; private set; }
    
    public AgentRunId AgentRunId { get; private set; }
    
    public IReadOnlyCollection<FunctionInvocation> Invocations => _invocations.AsReadOnly();
    private readonly List<FunctionInvocation> _invocations;

    private ExecutionPlan() { }

    internal ExecutionPlan(List<FunctionInvocation> invocations)
    {
        Id = new ExecutionPlanId(Guid.NewGuid());
        _invocations = invocations;
    }
}