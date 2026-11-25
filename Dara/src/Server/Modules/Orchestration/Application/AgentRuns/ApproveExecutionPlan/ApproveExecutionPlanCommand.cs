using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.ApproveExecutionPlan;

public record ApproveExecutionPlanCommand(Guid AgentRunId, Guid DeciderId) : ICommand;
