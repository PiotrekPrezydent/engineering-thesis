namespace Dara.Shared.Contracts.Authorization;

// http endpoints of Authorization for anyone with a session (a client token or the panel cookie): the roles the
// modules declare, the assignments of the caller, granting and revoking a role on one resource
public static class AuthorizationRoutes
{
    public const string Group = "/authorization";
    public const string Roles = Group + "/roles";
    public const string MyAssignments = Group + "/me/assignments";
    public const string Assignments = Group + "/assignments";
    public const string Revoke = Assignments + "/revoke";
}
