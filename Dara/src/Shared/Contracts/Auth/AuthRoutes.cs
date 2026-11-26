namespace Dara.Shared.Contracts.Auth;

// http endpoints of sign-in; login, device-login and refresh need no token, logout needs a device token or the
// cookie of the panel, the login code needs the cookie of the panel
public static class AuthRoutes
{
    public const string Group = "/auth";

    // the account panel: login and password, answered with the session cookie
    public const string Login = Group + "/login";
    // a device: the one-time code of the panel, answered with device tokens
    public const string DeviceLogin = Group + "/device-login";
    public const string Refresh = Group + "/refresh";
    public const string Logout = Group + "/logout";

    public const string AccountDevicesGroup = "/account/devices";

    public static string DeviceLoginCode(Guid deviceId) => $"{AccountDevicesGroup}/{deviceId}/login-code";
}
