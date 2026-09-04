using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.Participants;

public record ParticipantSnapshot(Guid ParticipantId, string Name, IReadOnlyList<FunctionSnapshot> Functions);