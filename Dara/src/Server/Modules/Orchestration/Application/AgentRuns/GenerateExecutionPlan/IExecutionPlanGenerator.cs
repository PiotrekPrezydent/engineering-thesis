namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// one loop for every provider, the provider and the model come with the request; a failure is a result, not an exception
public interface IExecutionPlanGenerator
{
    Task<ExecutionPlanGenerationResult> GenerateAsync(ExecutionPlanGenerationRequest request, IProposedCallRecorder recorder);
}
