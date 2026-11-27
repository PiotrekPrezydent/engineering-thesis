using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Plugins.Application;

// what a subject may do on one resource of a scope of the module, the code is "<scope path>.<action>"; the
// endpoints of the module are marked with them once the owner decides who may do what (TASK-ENG-040-D08)
public static class PluginsPermissions
{
    public static readonly PermissionDefinition Read = new("plugins.plugin.read", PluginsScopes.Plugin);
    public static readonly PermissionDefinition Configure = new("plugins.plugin.configure", PluginsScopes.Plugin);
    public static readonly PermissionDefinition ManageAccess = new(PluginsScopes.Plugin.ManageAccessPermissionCode, PluginsScopes.Plugin);
    public static readonly PermissionDefinition UseFunction = new("plugins.plugin.function.use", PluginsScopes.Function);
    public static readonly PermissionDefinition ManageFunctionAccess = new(PluginsScopes.Function.ManageAccessPermissionCode, PluginsScopes.Function);
}
