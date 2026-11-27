using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users;

public record UserId(Guid Value) : BaseEntityId(Value);