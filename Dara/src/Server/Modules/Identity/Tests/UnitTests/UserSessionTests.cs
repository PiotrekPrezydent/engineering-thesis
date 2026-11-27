using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.UserSessions;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;

namespace Dara.Server.Modules.Identity.UnitTests;

// the refresh token rules of the session of a client of the hub, which starts signed in with the registration of the
// client, and the kinds of sessions
public class UserSessionTests
{
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan Lifetime = TimeSpan.FromDays(30);
    private static readonly UserId UserId = new(Guid.NewGuid());

    private static UserSession Start(string tokenHash, DateTime now)
    {
        return UserSession.StartHub(new ClientId(Guid.NewGuid()), UserId, tokenHash, now, Lifetime);
    }

    [Fact]
    public void RefreshUsesUpTheTokenAndIssuesANewOne()
    {
        var session = Start("t1", Now);
        var version = session.Version;

        Assert.Equal(RefreshOutcome.Refreshed, session.Refresh("t1", "t2", Now.AddMinutes(1), Lifetime));

        Assert.NotNull(session.Tokens.Single(t => t.TokenHash == "t1").ConsumedAt);
        Assert.True(session.Tokens.Single(t => t.TokenHash == "t2").IsActive(Now.AddMinutes(1)));
        Assert.Equal(Now.AddMinutes(1), session.LastRefreshedAt);
        Assert.True(session.Version > version);
    }

    [Fact]
    public void AReusedTokenEndsTheSession()
    {
        var session = Start("t1", Now);
        session.Refresh("t1", "t2", Now, Lifetime);

        Assert.Equal(RefreshOutcome.Reused, session.Refresh("t1", "t3", Now, Lifetime));

        Assert.True(session.IsEnded);
        Assert.NotNull(session.Tokens.Single(t => t.TokenHash == "t2").RevokedAt);
        Assert.Equal(RefreshOutcome.Revoked, session.Refresh("t2", "t4", Now, Lifetime));
        Assert.DoesNotContain(session.Tokens, t => t.TokenHash == "t3" || t.TokenHash == "t4");
    }

    // every new token gets the full lifetime, so the session lives 30 days from the last refresh
    [Fact]
    public void ATokenExpiresThirtyDaysAfterItWasIssued()
    {
        var session = Start("t1", Now);
        var refreshedAt = Now.AddDays(20);
        session.Refresh("t1", "t2", refreshedAt, Lifetime);

        Assert.Equal(RefreshOutcome.Refreshed, session.Refresh("t2", "t3", refreshedAt.AddDays(29), Lifetime));
        Assert.Equal(RefreshOutcome.Expired, session.Refresh("t3", "t4", refreshedAt.AddDays(29) + Lifetime, Lifetime));
    }

    [Fact]
    public void SignOutRevokesTheTokensAndARepeatedOneChangesNothing()
    {
        var session = Start("t1", Now);

        session.SignOut(Now);
        var version = session.Version;
        session.SignOut(Now.AddMinutes(1));

        Assert.Equal(Now, session.EndedAt);
        Assert.Equal(version, session.Version);
        Assert.Equal(RefreshOutcome.Revoked, session.Refresh("t1", "t2", Now, Lifetime));
    }

    [Fact]
    public void AnUnknownTokenIsInvalid()
    {
        var session = Start("t1", Now);

        Assert.Equal(RefreshOutcome.Invalid, session.Refresh("other", "t2", Now, Lifetime));
        Assert.False(session.IsEnded);
    }

    // the active token and the newest used ones stay; a very old token is Invalid, not Reused
    [Fact]
    public void OnlyTheNewestUsedTokensAreKept()
    {
        var session = Start("t0", Now);
        for (var i = 1; i <= 8; i++)
            session.Refresh($"t{i - 1}", $"t{i}", Now.AddMinutes(i), Lifetime);

        Assert.Equal(UserSession.InactiveTokensKept + 1, session.Tokens.Count);
        Assert.Single(session.Tokens, t => t.IsActive(Now.AddMinutes(8)));
        Assert.Equal(RefreshOutcome.Invalid, session.Refresh("t0", "x", Now.AddMinutes(9), Lifetime));
        Assert.Equal(RefreshOutcome.Reused, session.Refresh("t7", "y", Now.AddMinutes(9), Lifetime));
    }

    [Fact]
    public void ASignInRevokesTheTokensOfTheEarlierCopyAndOpensTheSessionAgain()
    {
        var session = Start("t1", Now);
        session.SignOut(Now);

        session.SignIn("t2", Now.AddMinutes(1), Lifetime);

        Assert.False(session.IsEnded);
        Assert.Equal(RefreshOutcome.Revoked, session.Refresh("t1", "x", Now.AddMinutes(2), Lifetime));
        Assert.Equal(RefreshOutcome.Refreshed, session.Refresh("t2", "t3", Now.AddMinutes(2), Lifetime));
    }

    [Fact]
    public void ARevokedSessionEndsOnceAndCannotSignInAgain()
    {
        var session = Start("t1", Now);

        session.Revoke(Now);
        var version = session.Version;
        session.Revoke(Now.AddMinutes(1));

        Assert.Equal(Now, session.RevokedAt);
        Assert.Equal(version, session.Version);
        Assert.Equal(RefreshOutcome.Revoked, session.Refresh("t1", "t2", Now, Lifetime));
        var refused = Assert.Throws<BuisnessRuleValidationException>(() => session.SignIn("t2", Now, Lifetime));
        Assert.IsType<ClientMustNotBeRevokedRule>(refused.BrokenRule);
    }

    // a session of the hub has no sign-ins of browsers, the session of the panel has no refresh tokens
    [Fact]
    public void ASessionTakesOnlyTheChangesOfItsKind()
    {
        var hub = Start("t1", Now);
        var panel = UserSession.StartPanel(new ClientId(Guid.NewGuid()), UserId, Now);

        var refused = Assert.Throws<BuisnessRuleValidationException>(() => hub.StartPanelSignIn(Now, Lifetime));
        Assert.IsType<SessionMustMatchClientKindRule>(refused.BrokenRule);
        refused = Assert.Throws<BuisnessRuleValidationException>(() => panel.SignIn("t2", Now, Lifetime));
        Assert.IsType<SessionMustMatchClientKindRule>(refused.BrokenRule);
        refused = Assert.Throws<BuisnessRuleValidationException>(() => panel.Revoke(Now));
        Assert.IsType<SessionMustMatchClientKindRule>(refused.BrokenRule);
        Assert.Empty(panel.Tokens);
    }
}
