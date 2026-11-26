using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Sessions;

// a sign-in of the account panel with the password; its id lives in the cookie of the panel and the server checks
// the session on every request, so a sign-out works at once; an account may have several (several browsers);
// it is not a device, it has no refresh tokens and never connects to the hub
public class AccountSession : Entity, IAggregateRoot
{
    public AccountSessionId Id { get; private set; }
    public UserId UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    // the idle lifetime counts from here
    public DateTime LastUsedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    private AccountSession() { }

    private AccountSession(UserId userId, DateTime now)
    {
        Id = new AccountSessionId(Guid.NewGuid());
        UserId = userId;
        CreatedAt = now;
        LastUsedAt = now;
    }

    public bool IsEnded => EndedAt != null;

    public static AccountSession Start(UserId userId, DateTime now)
    {
        return new AccountSession(userId, now);
    }

    // false when the session ended or was not used for the idle lifetime; a valid use moves the idle lifetime on
    public bool Use(DateTime now, TimeSpan idleLifetime)
    {
        if (IsEnded || LastUsedAt + idleLifetime <= now)
            return false;

        LastUsedAt = now;
        return true;
    }

    // a repeated sign-out changes nothing
    public void SignOut(DateTime now)
    {
        EndedAt ??= now;
    }
}
