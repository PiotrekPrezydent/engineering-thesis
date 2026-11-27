namespace Dara.Server.Modules.Authorization.Application.Definitions.GetRoles;

// PermissionCodes: every permission of the role, those of its subordinate roles of the same scope too;
// CompanionRoleCodes: its subordinate roles of a child scope, passed along with their resource ids in a grant
public record RoleDto(string Code, string ScopeCode, string Module, List<string> PermissionCodes, List<string> ManageableRoleCodes,
    List<string> CompanionRoleCodes);
