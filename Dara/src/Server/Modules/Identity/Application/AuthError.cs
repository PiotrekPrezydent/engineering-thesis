namespace Dara.Server.Modules.Identity.Application;

public enum AuthError
{
    // an unknown login and a wrong password give the same error
    InvalidCredentials,
    // unknown, replaced, used or expired
    InvalidLoginCode,
    // no device session, or one of another account: the panel does not learn about devices of others
    DeviceNotFound,
    DeviceRevoked,
    RefreshInvalid,
    RefreshExpired,
    RefreshReused,
    RefreshRevoked
}
