namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// the index the model uses as {{result:N}}, or the error it can fix
public record ProposedCallResult(int? Index, string? Error);
