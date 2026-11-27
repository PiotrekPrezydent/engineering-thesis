namespace Dara.Shared.Contracts.Auth;

// claims of the client token beside the standard "sub" (user id), "jti", "iat" and "exp", and of the cookie of the
// account panel ("sub", "sid", "scope")
public static class AuthClaims
{
    public const string Subject = "sub";
    public const string Scope = "scope";
    // the client id of a client token
    public const string Client = "client";
    // the account session id of the panel cookie
    public const string Session = "sid";

    // a client token: the hub and the endpoints of a client
    public const string ScopeClient = "client";
    // the panel cookie: the endpoints of the account panel, never the hub
    public const string ScopeAccount = "account";
}
