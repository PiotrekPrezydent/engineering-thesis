namespace Dara.Server.BuildingBlocks.Application.Authorization;

// the kind of resource a permission is about, e.g. "Plugins.Plugin"; the code is a path from the module to the
// entity, so it says what the permissions of the scope are about; an assignment of a role names one resource of that
// kind by its id (ScopeId) or every resource of it (ScopeIds.All)
public sealed record ScopeDefinition(string Code)
{
    public const string ManageAccessAction = "manage_access";

    // the one permission that lets a subject grant and revoke roles in the scope: the path of the scope in lower case
    // and ".manage_access", e.g. "plugins.plugin.manage_access"; a scope without it takes assignments only from its module
    public string ManageAccessPermissionCode => ManageAccessPermissionCodeOf(Code);

    public static string ManageAccessPermissionCodeOf(string scopeCode)
    {
        return $"{scopeCode.ToLowerInvariant()}.{ManageAccessAction}";
    }
}
