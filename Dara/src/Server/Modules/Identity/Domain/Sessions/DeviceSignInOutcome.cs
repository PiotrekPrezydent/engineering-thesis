namespace Dara.Server.Modules.Identity.Domain.Sessions;

public enum DeviceSignInOutcome
{
    SignedIn,
    // unknown, replaced, used or expired: the panel issues a new code
    InvalidLoginCode,
    // a revoked device cannot sign in again
    DeviceRevoked
}
