using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Application;

// Owner may do everything in the module, Viewer only read; both are given to every new account and client
internal sealed class IdentityRoles : IModuleRoles
{
    public void Configure(ModuleRolesBuilder roles) => roles
        .Role(new IdentityRole(IdentityRole.Roles.Owner), isDefault: true)
            .AllowsAll<IdentityPermission>()
        .Role(new IdentityRole(IdentityRole.Roles.Viewer), isDefault: true)
            .Allows(new IdentityPermission(IdentityPermission.Permissions.ReadClients));
}
