using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Domain.UserSessions;

// the sessions of the user through one client, one per client, started with the client: a client of the hub has
// refresh tokens (rotation on every refresh, the full lifetime for every new token, the end of the session when a
// used token comes back), the panel client has the sign-ins of its browsers (a cookie each, an idle lifetime);
// refusals are results, not exceptions, because a change of state (an ended session) must be saved and the unit of
// work saves nothing after an exception; a separate aggregate from Client for its concurrency token
public class UserSession : Entity, IAggregateRoot
{
    // used and revoked tokens kept to recognize a reuse; older ones are removed, so a very old token is Invalid
    public const int InactiveTokensKept = 5;

    private readonly List<RefreshToken> _tokens = new();
    private readonly List<PanelSignIn> _panelSignIns = new();

    public UserSessionId Id { get; private set; }
    public ClientId ClientId { get; private set; }
    public UserId UserId { get; private set; }
    public ClientKind Kind { get; private set; }
    public DateTime CreatedAt { get; private set; }
    // the hub: the last token issued
    public DateTime LastRefreshedAt { get; private set; }
    // the hub: sign-out, revocation or a reused token
    public DateTime? EndedAt { get; private set; }
    // the hub: the client was revoked in its account; the session cannot sign in again
    public DateTime? RevokedAt { get; private set; }

    // concurrency token of the refresh tokens: of two parallel refreshes with one token only one is saved; the
    // sign-ins of the panel do not raise it, so parallel requests of the panel (each moves its idle lifetime) never
    // conflict
    public int Version { get; private set; }

    public IReadOnlyCollection<RefreshToken> Tokens => _tokens;

    public IReadOnlyCollection<PanelSignIn> PanelSignIns => _panelSignIns;

    public bool IsEnded => EndedAt != null;

    public bool IsRevoked => RevokedAt != null;

    private UserSession() { }

    private UserSession(ClientId clientId, UserId userId, ClientKind kind, DateTime now)
    {
        Id = new UserSessionId(Guid.NewGuid());
        ClientId = clientId;
        UserId = userId;
        Kind = kind;
        CreatedAt = now;
        LastRefreshedAt = now;
        Version = 1;
    }

    // the registration of a client of the hub gives its first refresh token at once
    public static UserSession StartHub(ClientId clientId, UserId userId, string tokenHash, DateTime now, TimeSpan lifetime)
    {
        var session = new UserSession(clientId, userId, ClientKind.Hub, now);
        session.IssueToken(tokenHash, now, lifetime);
        return session;
    }

    // created with the panel client of a new account, without any sign-in
    public static UserSession StartPanel(ClientId clientId, UserId userId, DateTime now)
    {
        return new UserSession(clientId, userId, ClientKind.Panel, now);
    }

    // the hub, after a login code of the panel: the tokens of the earlier copy of the app end and a signed-out
    // session opens again; Client refuses the code of a revoked client, so a revoked session here is a bug of the
    // caller
    public void SignIn(string tokenHash, DateTime now, TimeSpan lifetime)
    {
        CheckRule(new SessionMustMatchClientKindRule(Kind, ClientKind.Hub));
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

    // the hub; a repeated sign-out changes nothing
    public void SignOut(DateTime now)
    {
        if (IsEnded)
            return;

        End(now);
    }

    // the hub; a repeated revocation changes nothing
    public void Revoke(DateTime now)
    {
        CheckRule(new SessionMustMatchClientKindRule(Kind, ClientKind.Hub));
        if (IsRevoked)
            return;

        RevokedAt = now;
        SignOut(now);
        Changed();
    }

    // the panel, after the password: every sign-in is a new one (another browser); ended and idle sign-ins go here
    public PanelSignIn StartPanelSignIn(DateTime now, TimeSpan idleLifetime)
    {
        CheckRule(new SessionMustMatchClientKindRule(Kind, ClientKind.Panel));

        _panelSignIns.RemoveAll(s => s.IsEnded || s.IsIdle(now, idleLifetime));

        var signIn = PanelSignIn.Start(now);
        _panelSignIns.Add(signIn);
        return signIn;
    }

    // false when the sign-in is unknown, ended or idle; a valid use moves its idle lifetime on
    public bool UsePanelSignIn(PanelSignInId signInId, DateTime now, TimeSpan idleLifetime)
    {
        var signIn = _panelSignIns.FirstOrDefault(s => s.Id == signInId);
        return signIn != null && signIn.Use(now, idleLifetime);
    }

    // an unknown sign-in is left as it is; a repeated sign-out changes nothing
    public void SignOutPanelSignIn(PanelSignInId signInId, DateTime now)
    {
        _panelSignIns.FirstOrDefault(s => s.Id == signInId)?.SignOut(now);
    }

    private void IssueToken(string tokenHash, DateTime now, TimeSpan lifetime)
    {
        CheckRule(new SessionMustMatchClientKindRule(Kind, ClientKind.Hub));

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
