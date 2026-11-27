using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Authorization.Application.Users.CheckPermissions;

// which of the permission codes the caller has; the host asks it before a marked endpoint or hub method
public record CheckPermissionsQuery(Guid UserId, Guid? ClientId, List<string> PermissionCodes) : IQuery<List<PermissionCheckDto>>;
