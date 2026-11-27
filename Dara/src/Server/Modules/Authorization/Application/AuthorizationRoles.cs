using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.Modules.Authorization.Domain;

namespace Dara.Server.Modules.Authorization.Application;

// Owner may also give and take roles, Viewer only reads the roles and its own permissions; both are given to every
// new account and client
internal sealed class AuthorizationRoles : IModuleRoles
{
    public void Configure(ModuleRolesBuilder roles) => roles
        .Role(new AuthorizationRole(AuthorizationRole.Roles.Owner), isDefault: true)
            .AllowsAll<AuthorizationPermission>()
        .Role(new AuthorizationRole(AuthorizationRole.Roles.Viewer), isDefault: true)
            .Allows(
                new AuthorizationPermission(AuthorizationPermission.Permissions.GetRoles),
                new AuthorizationPermission(AuthorizationPermission.Permissions.GetPermissions),
                new AuthorizationPermission(AuthorizationPermission.Permissions.CheckPermissions));
}
