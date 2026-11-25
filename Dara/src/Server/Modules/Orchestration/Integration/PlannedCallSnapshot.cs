namespace Dara.Server.Modules.Orchestration.Integration;

// a call of the plan in a readable form, the arguments as one json object with {{result:N}} for results of earlier calls
public record PlannedCallSnapshot(int Index, Guid ParticipantId, string ParticipantName, Guid FunctionId, string FunctionName, string ArgumentsJson);
