namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

public record FunctionSnapshot(Guid FunctionId, Guid ParticipantId, string Name, string Description, string ReturnTypeName, IReadOnlyList<FunctionParameter> Parameters);