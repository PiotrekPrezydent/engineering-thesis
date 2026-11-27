using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Roles;

public record RoleId(Guid Value) : BaseEntityId(Value);
