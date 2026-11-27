using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Users;

public record UserRolesId(Guid Value) : BaseEntityId(Value);
