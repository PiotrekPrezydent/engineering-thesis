using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.Modules.Plugins.Domain;

namespace Dara.Server.Modules.Plugins.Application;

// Owner may do everything in the module, Viewer only read; both are given to every new account and client
internal sealed class PluginsRoles : IModuleRoles
{
    public void Configure(ModuleRolesBuilder roles) => roles
        .Role(new PluginsRole(PluginsRole.Roles.Owner), isDefault: true)
            .AllowsAll<PluginsPermission>()
        .Role(new PluginsRole(PluginsRole.Roles.Viewer), isDefault: true)
            .Allows(new PluginsPermission(PluginsPermission.Permissions.ReadPlugins));
}
