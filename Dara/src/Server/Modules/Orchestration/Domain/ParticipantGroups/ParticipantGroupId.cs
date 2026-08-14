using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;

public record ParticipantGroupId(Guid Value) : BaseEntityId(Value);