namespace Dara.Shared.Contracts.Authorization;

// ScopeId: the id of the resource (e.g. a user, a client, a plugin); Guid.Empty: every resource of the scope (a global role)
public record RoleAssignmentSnapshot(Guid SubjectId, string RoleCode, string ScopeCode, Guid ScopeId, IReadOnlyList<string> PermissionCodes);
