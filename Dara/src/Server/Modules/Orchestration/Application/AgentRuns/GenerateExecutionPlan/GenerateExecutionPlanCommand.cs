using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

public record GenerateExecutionPlanCommand(Guid AgentRunId) : ICommand;
