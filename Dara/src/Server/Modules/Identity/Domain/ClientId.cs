using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain;

public record ClientId(Guid Value) : BaseEntityId(Value);