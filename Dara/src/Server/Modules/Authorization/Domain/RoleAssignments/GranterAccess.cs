using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments;

// what the subject that grants or revokes a role holds on the resource of the assignment (its roles of the scope
// there and its global ones): the permission codes and the manageable role codes of those roles, the code of the
// permission that manages access in the scope, and whether it is the administrator of the server (above the rules)
public sealed record GranterAccess(
    IReadOnlySet<string> PermissionCodes,
    IReadOnlySet<string> ManageableRoleCodes,
    string ManageAccessPermissionCode,
    bool IsServerAdmin) : IValueObject
{
    public bool ManagesAccess => IsServerAdmin || PermissionCodes.Contains(ManageAccessPermissionCode);

    public bool Manages(string roleCode) => IsServerAdmin || ManageableRoleCodes.Contains(roleCode);
}
