using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Permissions;

public record PermissionId(Guid Value) : BaseEntityId(Value);
