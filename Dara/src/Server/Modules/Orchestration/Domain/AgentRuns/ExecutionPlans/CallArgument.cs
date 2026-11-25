using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

// one argument of a planned call as json text, {{result:N}} stands for the result of call N
public record CallArgument(string Name, CallArgumentKind Kind, string Json, IReadOnlyList<int> ResultIndexes) : IValueObject;
