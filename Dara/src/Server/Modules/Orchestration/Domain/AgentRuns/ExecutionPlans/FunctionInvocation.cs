namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

public record FunctionInvocation(Guid FunctionId, IDictionary<string,string> ParametersArgumentsMap);