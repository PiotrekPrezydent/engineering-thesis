using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.RejectExecutionPlan;

public record RejectExecutionPlanCommand(Guid AgentRunId, Guid DeciderId) : ICommand;
