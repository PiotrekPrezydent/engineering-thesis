namespace Dara.Shared.Contracts.Authorization;

// a grant or a revoke: the subject is any id (a user, a client), the scope follows from the role; Companions: the
// companion roles of the role on their own resources (e.g. Plugins.FunctionOwner on each function of the plugin),
// all or nothing
public record RoleAssignmentRequest(Guid SubjectId, string RoleCode, Guid ScopeId, IReadOnlyList<CompanionAssignmentRequest>? Companions = null);

public record CompanionAssignmentRequest(string RoleCode, Guid ScopeId);
