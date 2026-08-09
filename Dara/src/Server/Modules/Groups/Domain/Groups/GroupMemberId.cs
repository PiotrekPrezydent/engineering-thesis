using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public record GroupMemberId(Guid Value) : BaseEntityId(Value);