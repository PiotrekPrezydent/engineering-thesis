namespace Dara.Server.Modules.Identity.Domain.Sessions;

public enum RefreshOutcome
{
    Refreshed,
    // the session ended or the token was revoked
    Revoked,
    // the token was already exchanged once: someone else may have it, the whole session is ended
    Reused,
    Expired,
    // the session does not know the token (also a token removed by the cleanup)
    Invalid
}
