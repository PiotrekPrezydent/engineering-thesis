using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.Modules.Groups.Domain;

namespace Dara.Server.Modules.Groups.Application;

// Owner may do everything in the module, Viewer only read; both are given to every new account and client
internal sealed class GroupsRoles : IModuleRoles
{
    public void Configure(ModuleRolesBuilder roles) => roles
        .Role(new GroupsRole(GroupsRole.Roles.Owner), isDefault: true)
            .AllowsAll<GroupsPermission>()
        .Role(new GroupsRole(GroupsRole.Roles.Viewer), isDefault: true)
            .Allows(new GroupsPermission(GroupsPermission.Permissions.ReadGroups));
}
