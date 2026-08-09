using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public record GroupId(Guid Value) : BaseEntityId(Value);