namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// succeeded when the model ended with calls the run accepted and left no call with an error
public record ExecutionPlanGenerationResult(bool Succeeded, string? FailureReason = null);
