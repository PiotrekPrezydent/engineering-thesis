using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Plugins.Application;

public static class PluginsRoles
{
    private const string OwnerCode = "Plugins.Owner";
    private const string ManagerCode = "Plugins.Manager";
    private const string FunctionOwnerCode = "Plugins.FunctionOwner";

    // sees the plugin; every new user and client has it on every plugin (the plugins of the server)
    public static readonly RoleDefinition Viewer = new("Plugins.Viewer", PluginsScopes.Plugin, [PluginsPermissions.Read]);

    // configures
    public static readonly RoleDefinition Editor = new("Plugins.Editor", PluginsScopes.Plugin, [PluginsPermissions.Configure])
    {
        Subordinates = [Viewer]
    };

    // passes access on, but cannot make or remove an owner
    public static readonly RoleDefinition Manager = new(ManagerCode, PluginsScopes.Plugin, [PluginsPermissions.ManageAccess])
    {
        Subordinates = [Editor],
        ManageableRoleCodes = [ManagerCode, Editor.Code, Viewer.Code]
    };

    // holds one function of a plugin (ScopeId = the function id)
    public static readonly RoleDefinition FunctionOwner = new(FunctionOwnerCode, PluginsScopes.Function, [PluginsPermissions.UseFunction, PluginsPermissions.ManageFunctionAccess])
    {
        ManageableRoleCodes = [FunctionOwnerCode]
    };

    // everything a manager may, the owners, and each function of the plugin: a grant of the owner passes
    // FunctionOwner with the id of every function along (a companion role of the child scope)
    public static readonly RoleDefinition Owner = new(OwnerCode, PluginsScopes.Plugin, [])
    {
        Subordinates = [Manager, FunctionOwner],
        ManageableRoleCodes = [OwnerCode, ManagerCode, Editor.Code, Viewer.Code]
    };

    public static readonly IReadOnlyList<NewSubjectRole> ForNewSubjects = [new(Viewer)];
}
