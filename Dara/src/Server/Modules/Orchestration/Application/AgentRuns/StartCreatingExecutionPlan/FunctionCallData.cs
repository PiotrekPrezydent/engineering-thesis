namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartCreatingExecutionPlan;

public record FunctionCallData(string FunctionName, IDictionary<string, string> Parameters);