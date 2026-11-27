using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.UserSessions;

// one sign-in of a browser into the account panel with the password; its id lives in the cookie of the panel and the
// server checks it on every request, so a sign-out works at once; a panel may have several (several browsers), all
// acting as the one panel client of the account
public class PanelSignIn : Entity
{
    public PanelSignInId Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    // the idle lifetime counts from here
    public DateTime LastUsedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    public bool IsEnded => EndedAt != null;

    private PanelSignIn() { }

    private PanelSignIn(DateTime now)
    {
        Id = new PanelSignInId(Guid.NewGuid());
        CreatedAt = now;
        LastUsedAt = now;
    }

    internal static PanelSignIn Start(DateTime now)
    {
        return new PanelSignIn(now);
    }

    public bool IsIdle(DateTime now, TimeSpan idleLifetime)
    {
        return LastUsedAt + idleLifetime <= now;
    }

    // false when the sign-in ended or was not used for the idle lifetime; a valid use moves the idle lifetime on
    internal bool Use(DateTime now, TimeSpan idleLifetime)
    {
        if (IsEnded || IsIdle(now, idleLifetime))
            return false;

        LastUsedAt = now;
        return true;
    }

    // a repeated sign-out changes nothing
    internal void SignOut(DateTime now)
    {
        EndedAt ??= now;
    }
}
