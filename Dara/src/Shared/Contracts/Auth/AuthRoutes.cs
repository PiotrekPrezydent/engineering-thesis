namespace Dara.Shared.Contracts.Auth;

// http endpoints of sign-in; login, register-client, client-login and refresh need no token, logout needs a client
// token or the cookie of the panel, the codes of the panel need the cookie of the panel
public static class AuthRoutes
{
    public const string Group = "/auth";

    // the account panel: login and password, answered with the session cookie
    public const string Login = Group + "/login";
    // the app of a new client: the registration code of the panel, answered with client tokens and the client id
    public const string RegisterClient = Group + "/register-client";
    // an existing client: the login code of the panel, answered with client tokens
    public const string ClientLogin = Group + "/client-login";
    public const string Refresh = Group + "/refresh";
    public const string Logout = Group + "/logout";

    public const string AccountClientsGroup = "/account/clients";

    // a code for a new client of the account
    public const string RegistrationCode = AccountClientsGroup + "/registration-code";

    public static string ClientLoginCode(Guid clientId) => $"{AccountClientsGroup}/{clientId}/login-code";
}
