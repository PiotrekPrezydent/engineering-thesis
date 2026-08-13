using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants;

public record ParticipantId(Guid Value) : BaseEntityId(Value);