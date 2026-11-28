namespace Dara.Shared.Contracts.Errors;

// the "code" field of the problem details of a refused request of the endpoints of groups, agent runs, agent settings
// and plugins; a broken rule also names the rule in the "rule" field
public static class ApiErrorCodes
{
    // 400, the detail says which rule
    public const string RuleBroken = Auth.AuthErrorCodes.RuleBroken;
    // 403, the rule of the module refuses the caller (not a member, not the owner, not the author, a wrong join code)
    public const string Forbidden = "forbidden";
    // 404, no such resource, or one the caller may not see
    public const string NotFound = "not_found";
    // 409, the state of the resource takes no such change (already a member, already enabled, plan decided)
    public const string Conflict = "conflict";
}
