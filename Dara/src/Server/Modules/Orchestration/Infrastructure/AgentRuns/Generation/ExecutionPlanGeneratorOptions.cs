namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;

// limits of one plan generation, the loop of microsoft.extensions.ai would allow 40 rounds
public record ExecutionPlanGeneratorOptions(int TimeoutSeconds = 60, int MaximumIterations = 16);
