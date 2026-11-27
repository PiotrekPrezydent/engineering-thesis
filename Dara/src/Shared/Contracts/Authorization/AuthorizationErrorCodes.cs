namespace Dara.Shared.Contracts.Authorization;

// the "code" field of the problem details of a refused request of /authorization/* and of an endpoint marked with a
// permission; a hub method marked with a permission fails with a HubException whose message is Forbidden
public static class AuthorizationErrorCodes
{
    // 400, a broken rule also names the rule in the "rule" field
    public const string RuleBroken = "rule_broken";
    // 400, the route value with the id of the resource is not a guid
    public const string InvalidScopeId = "invalid_scope_id";
    // 400, a role on every resource of a scope (ScopeId Guid.Empty) is granted and revoked only automatically
    public const string GlobalAssignment = "global_assignment";
    // 403: no assignment allows it (default deny), or the caller does not manage access there or holds no role that manages the role
    public const string Forbidden = "forbidden";
    // 404
    public const string RoleNotFound = "role_not_found";
    // 404
    public const string AssignmentNotFound = "assignment_not_found";
    // 409
    public const string AlreadyAssigned = "already_assigned";
}
