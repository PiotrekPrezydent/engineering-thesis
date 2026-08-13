using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants;

public record ParticipantGroupId(Guid Value) : BaseEntityId(Value);