namespace Dara.Shared.Contracts.StateSnapshots;

// a call of the plan, the arguments as one json object with {{result:N}} for results of earlier calls
public record PlannedCallSnapshot(int Index, Guid ParticipantId, string ParticipantName, Guid FunctionId, string FunctionName, string ArgumentsJson);
