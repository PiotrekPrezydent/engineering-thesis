using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain;

public record ClientIdentityId(Guid Value) : BaseEntityId(Value);