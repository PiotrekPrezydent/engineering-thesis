namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.GetMyAssignments;

// ScopeId Guid.Empty: on every resource of the scope (a global role, "*")
public record RoleAssignmentDto(Guid SubjectId, string RoleCode, string ScopeCode, Guid ScopeId, List<string> PermissionCodes);
