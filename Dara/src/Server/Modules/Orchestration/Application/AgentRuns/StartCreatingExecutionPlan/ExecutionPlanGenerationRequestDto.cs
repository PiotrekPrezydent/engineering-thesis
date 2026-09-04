namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartCreatingExecutionPlan;

public record ExecutionPlanGenerationRequestDto(string ApiKey, string ModelName, IReadOnlyList<FunctionData> Functions);