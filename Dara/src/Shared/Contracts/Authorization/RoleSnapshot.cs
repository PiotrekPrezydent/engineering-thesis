namespace Dara.Shared.Contracts.Authorization;

// PermissionCodes: every permission of the role, of its subordinate roles of the same scope too; ManageableRoleCodes:
// the roles its holder may grant and revoke on the same resource; CompanionRoleCodes: its subordinate roles of a
// child scope, which a grant or revoke of the role passes along with their resource ids (RoleAssignmentRequest.Companions)
public record RoleSnapshot(string Code, string ScopeCode, string Module, IReadOnlyList<string> PermissionCodes,
    IReadOnlyList<string> ManageableRoleCodes, IReadOnlyList<string> CompanionRoleCodes);
