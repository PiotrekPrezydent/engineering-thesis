namespace Dara.Shared.Contracts.Auth;

// claims of the device token beside the standard "sub" (user id), "jti", "iat" and "exp", and of the cookie of the
// account panel ("sub", "sid", "scope")
public static class AuthClaims
{
    public const string Subject = "sub";
    public const string Scope = "scope";
    // the device id of a device token
    public const string Device = "device";
    // the account session id of the panel cookie
    public const string Session = "sid";

    // a device token: the hub and the endpoints of a device
    public const string ScopeDevice = "device";
    // the panel cookie: the endpoints of the account panel, never the hub
    public const string ScopeAccount = "account";
}
