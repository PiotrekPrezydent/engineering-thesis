namespace Dara.Server.BuildingBlocks.Application.Authorization;

// what a subject may do on one resource of the scope; the code starts with the path of the scope, e.g.
// "plugins.plugin.configure"; permissions only allow, everything else is denied
public sealed record PermissionDefinition(string Code, ScopeDefinition Scope)
{
    // the permission that manages access in its scope, known by its code alone (ScopeDefinition.ManageAccessPermissionCode)
    public bool ManagesAccess => string.Equals(Code, Scope.ManageAccessPermissionCode, StringComparison.Ordinal);
}
