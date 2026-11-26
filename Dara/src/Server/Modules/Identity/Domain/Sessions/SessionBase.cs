using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Sessions;

// the refresh token rules of a device session: rotation on every refresh, the full lifetime for every new
// token and the end of the session when a used token comes back; refusals are results, not exceptions, because
// a change of state (an ended session) must be saved and the unit of work saves nothing after an exception
public abstract class SessionBase : Entity
{
    // used and revoked tokens kept to recognize a reuse; older ones are removed, so a very old token is Invalid
    public const int InactiveTokensKept = 5;

    private readonly List<RefreshToken> _tokens = new();

    public UserId UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastRefreshedAt { get; private set; }
    // sign-out, revocation or a reused token
    public DateTime? EndedAt { get; protected set; }

    // concurrency token: of two parallel refreshes with one token only one is saved
    public int Version { get; private set; }

    public IReadOnlyCollection<RefreshToken> Tokens => _tokens;

    protected SessionBase() { }

    protected SessionBase(UserId userId, DateTime now)
    {
        UserId = userId;
        CreatedAt = now;
        LastRefreshedAt = now;
        Version = 1;
    }

    public bool IsEnded => EndedAt != null;

    public RefreshOutcome Refresh(string presentedHash, string newHash, DateTime now, TimeSpan lifetime)
    {
        var token = _tokens.FirstOrDefault(t => t.TokenHash == presentedHash);
        if (token == null)
            return RefreshOutcome.Invalid;

        if (IsEnded || token.RevokedAt != null)
            return RefreshOutcome.Revoked;

        if (token.ConsumedAt != null)
        {
            End(now);
            return RefreshOutcome.Reused;
        }

        if (token.IsExpired(now))
            return RefreshOutcome.Expired;

        token.Consume(now);
        IssueToken(newHash, now, lifetime);
        return RefreshOutcome.Refreshed;
    }

    // a repeated sign-out changes nothing
    public void SignOut(DateTime now)
    {
        if (IsEnded)
            return;

        End(now);
    }

    protected void IssueToken(string tokenHash, DateTime now, TimeSpan lifetime)
    {
        _tokens.Add(new RefreshToken(tokenHash, now, now + lifetime));
        LastRefreshedAt = now;
        RemoveOldTokens(now);
        Changed();
    }

    protected void RevokeActiveTokens(DateTime now)
    {
        foreach (var token in _tokens.Where(t => t.ConsumedAt == null && t.RevokedAt == null))
            token.Revoke(now);

        Changed();
    }

    protected void Changed()
    {
        Version++;
    }

    private void End(DateTime now)
    {
        RevokeActiveTokens(now);
        EndedAt = now;
    }

    // active tokens stay, expired ones go, of the used and revoked ones the newest few stay
    private void RemoveOldTokens(DateTime now)
    {
        var removed = _tokens.Where(t => t.IsExpired(now)).ToList();
        removed.AddRange(_tokens
            .Where(t => !t.IsExpired(now) && !t.IsActive(now))
            .OrderByDescending(t => t.IssuedAt)
            .Skip(InactiveTokensKept));

        foreach (var token in removed)
            _tokens.Remove(token);
    }
}
