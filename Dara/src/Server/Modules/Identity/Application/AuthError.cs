namespace Dara.Server.Modules.Identity.Application;

public enum AuthError
{
    // an unknown login and a wrong password give the same error
    InvalidCredentials,
    // unknown, used or expired
    InvalidRegistrationCode,
    // unknown, replaced, used or expired
    InvalidLoginCode,
    // no client, or one of another account: the panel does not learn about clients of others
    ClientNotFound,
    ClientRevoked,
    RefreshInvalid,
    RefreshExpired,
    RefreshReused,
    RefreshRevoked
}
