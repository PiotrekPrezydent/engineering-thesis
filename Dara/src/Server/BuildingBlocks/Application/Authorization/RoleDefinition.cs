namespace Dara.Server.BuildingBlocks.Application.Authorization;

// a named set of permissions of one scope, e.g. "Plugins.Editor"; subjects get roles, never single permissions;
// Subordinates: roles this role stands above, e.g. Plugins.Owner over Plugins.Manager; one of the same scope gives
// this role its permissions (recursively) without declaring them; one of a child scope (its code is the code of
// this scope, a dot and more, e.g. Plugins.Plugin.Function under Plugins.Plugin) is a companion role: it holds on
// other resources (each function has its own id), so it is never flattened and a grant of this role through the
// api passes it along with the ids of those resources;
// ManageableRoleCodes: the roles of the same scope a holder of this role may grant and revoke on the same resource
// (an owner all of them, a manager all but the owner)
public sealed record RoleDefinition(string Code, ScopeDefinition Scope, IReadOnlyCollection<PermissionDefinition> Permissions)
{
    public IReadOnlyCollection<RoleDefinition> Subordinates { get; init; } = [];

    public IReadOnlyCollection<string> ManageableRoleCodes { get; init; } = [];

    // its own permissions and those of its subordinates of the same scope, recursively; a cycle (an invalid
    // definition) ends the walk
    public IReadOnlyList<PermissionDefinition> AllPermissions()
    {
        var permissions = new List<PermissionDefinition>();
        Walk(this, permissions, new List<RoleDefinition>(), new HashSet<string>(StringComparer.Ordinal));
        return permissions.Distinct().ToList();
    }

    // the subordinates of a child scope, of this role and of its subordinates of the same scope
    public IReadOnlyList<RoleDefinition> CompanionRoles()
    {
        var companions = new List<RoleDefinition>();
        Walk(this, new List<PermissionDefinition>(), companions, new HashSet<string>(StringComparer.Ordinal));
        return companions.DistinctBy(r => r.Code, StringComparer.Ordinal).ToList();
    }

    private void Walk(RoleDefinition role, List<PermissionDefinition> permissions, List<RoleDefinition> companions, HashSet<string> visited)
    {
        if (!visited.Add(role.Code))
            return;

        permissions.AddRange(role.Permissions);
        foreach (var subordinate in role.Subordinates)
        {
            if (subordinate.Scope.Code == Scope.Code)
                Walk(subordinate, permissions, companions, visited);
            else
                companions.Add(subordinate);
        }
    }
}
