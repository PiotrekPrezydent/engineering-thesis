using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Plugins.Application;

// the authorization definition of the module, checked at start-up and synchronized into Authorization
internal sealed class PluginsAuthorizationDefinition : IModuleAuthorizationDefinition
{
    public string ModuleName => "Plugins";

    public IEnumerable<ScopeDefinition> GetScopes() => [PluginsScopes.Plugin, PluginsScopes.Function];

    public IEnumerable<PermissionDefinition> GetPermissions() => [PluginsPermissions.Read, PluginsPermissions.Configure, PluginsPermissions.ManageAccess, PluginsPermissions.UseFunction, PluginsPermissions.ManageFunctionAccess];

    public IEnumerable<RoleDefinition> GetDefaultRoles() => [PluginsRoles.Owner, PluginsRoles.Manager, PluginsRoles.Editor, PluginsRoles.Viewer, PluginsRoles.FunctionOwner];

    public IEnumerable<NewSubjectRole> GetNewPanelRoles() => PluginsRoles.ForNewSubjects;

    public IEnumerable<NewSubjectRole> GetNewClientRoles() => PluginsRoles.ForNewSubjects;
}
