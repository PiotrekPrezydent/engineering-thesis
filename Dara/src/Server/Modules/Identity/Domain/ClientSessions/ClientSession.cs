using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Clients;
using Dara.Server.Modules.Identity.Domain.Users.Rules;

namespace Dara.Server.Modules.Identity.Domain.ClientSessions;

// the refresh tokens of one client of an account, one session per client, started when the client registers;
// rotation on every refresh, the full lifetime for every new token and the end of the session when a used token
// comes back; refusals are results, not exceptions, because a change of state (an ended session) must be saved and
// the unit of work saves nothing after an exception; a separate aggregate from User for its concurrency token
public class ClientSession : Entity, IAggregateRoot
{
    // used and revoked tokens kept to recognize a reuse; older ones are removed, so a very old token is Invalid
    public const int InactiveTokensKept = 5;

    private readonly List<RefreshToken> _tokens = new();

    public ClientSessionId Id { get; private set; }
    public UserClientId ClientId { get; private set; }
    public UserId UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastRefreshedAt { get; private set; }
    // sign-out, revocation or a reused token
    public DateTime? EndedAt { get; private set; }
    // the client was revoked in its account; the session cannot sign in again
    public DateTime? RevokedAt { get; private set; }

    // concurrency token: of two parallel refreshes with one token only one is saved
    public int Version { get; private set; }

    public IReadOnlyCollection<RefreshToken> Tokens => _tokens;

    public bool IsEnded => EndedAt != null;

    public bool IsRevoked => RevokedAt != null;

    private ClientSession() { }

    private ClientSession(UserClientId clientId, UserId userId, DateTime now)
    {
        Id = new ClientSessionId(Guid.NewGuid());
        ClientId = clientId;
        UserId = userId;
        CreatedAt = now;
        LastRefreshedAt = now;
        Version = 1;
    }

    // the registration of the client gives its first refresh token at once
    public static ClientSession Start(UserClientId clientId, UserId userId, string tokenHash, DateTime now, TimeSpan lifetime)
    {
        var session = new ClientSession(clientId, userId, now);
        session.IssueToken(tokenHash, now, lifetime);
        return session;
    }

    // after a login code of the panel: the tokens of the earlier copy of the app end and a signed-out session opens
    // again; User refuses the code of a revoked client, so a revoked session here is a bug of the caller
    public void SignIn(string tokenHash, DateTime now, TimeSpan lifetime)
    {
        CheckRule(new ClientMustNotBeRevokedRule(IsRevoked));

        RevokeActiveTokens(now);
        EndedAt = null;
        IssueToken(tokenHash, now, lifetime);
    }

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

    // a repeated revocation changes nothing
    public void Revoke(DateTime now)
    {
        if (IsRevoked)
            return;

        RevokedAt = now;
        SignOut(now);
        Changed();
    }

    private void IssueToken(string tokenHash, DateTime now, TimeSpan lifetime)
    {
        _tokens.Add(RefreshToken.Create(tokenHash, now, lifetime));
        LastRefreshedAt = now;
        RemoveOldTokens(now);
        Changed();
    }

    private void RevokeActiveTokens(DateTime now)
    {
        foreach (var token in _tokens.Where(t => t.ConsumedAt == null && t.RevokedAt == null))
            token.Revoke(now);

        Changed();
    }

    private void Changed()
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
