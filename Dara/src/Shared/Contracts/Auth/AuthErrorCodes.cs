namespace Dara.Shared.Contracts.Auth;

// the "code" field of the problem details of a refused auth request
public static class AuthErrorCodes
{
    // 400, the detail says which rule
    public const string RuleBroken = "rule_broken";
    // 401, an unknown login and a wrong password look the same
    public const string InvalidCredentials = "invalid_credentials";
    // 401, the registration code is unknown, used or expired: the panel creates a new one
    public const string InvalidRegistrationCode = "invalid_registration_code";
    // 401, the login code is unknown, replaced, used or expired: the panel issues a new one
    public const string InvalidLoginCode = "invalid_login_code";
    // 404, no such client of the signed-in account
    public const string ClientNotFound = "client_not_found";
    // 409, the client was revoked: it gets no code, takes no change and cannot sign in
    public const string ClientRevoked = "client_revoked";

    // 401, the refresh token gives no new access token: sign in with a new code from the panel
    public const string RefreshInvalid = "refresh_invalid";
    public const string RefreshExpired = "refresh_expired";
    public const string RefreshReused = "refresh_reused";
    public const string RefreshRevoked = "refresh_revoked";
}
