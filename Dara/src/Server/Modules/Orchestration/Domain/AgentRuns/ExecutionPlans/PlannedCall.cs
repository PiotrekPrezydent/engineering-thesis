using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

// a call of the plan by ids; the alias stays only to show the plan as the model wrote it
public record PlannedCall(int Index, string Alias, ParticipantId ParticipantId, FunctionId FunctionId, IReadOnlyList<CallArgument> Arguments) : IValueObject;
