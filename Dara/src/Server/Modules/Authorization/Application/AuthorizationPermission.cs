using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Authorization.Application;

// what a user may do in the Authorization module; the endpoints of Authorization are marked with it
public sealed record AuthorizationPermission(AuthorizationPermission.Permissions Value) : ModulePermission<AuthorizationPermission.Permissions>(Value)
{
    public enum Permissions
    {
        GetRoles,
        GetPermissions,
        CheckPermissions,
        AddRole,
        RemoveRole
    }
}
