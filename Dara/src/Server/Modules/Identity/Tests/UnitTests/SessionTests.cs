using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.UnitTests;

// the refresh token rules of SessionBase on a signed-in device
public class SessionTests
{
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan Lifetime = TimeSpan.FromDays(30);
    private static readonly UserId UserId = new(Guid.NewGuid());

    // a device created in the panel and signed in with a code, its first refresh token is tokenHash
    private static DeviceSession Start(string tokenHash, DateTime now)
    {
        var session = DeviceSession.Register(Guid.NewGuid(), UserId, now);
        session.IssueLoginCode("code", now, TimeSpan.FromMinutes(10));
        Assert.Equal(DeviceSignInOutcome.SignedIn, session.SignIn("code", tokenHash, now, Lifetime));
        return session;
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

        Assert.Equal(SessionBase.InactiveTokensKept + 1, session.Tokens.Count);
        Assert.Single(session.Tokens, t => t.IsActive(Now.AddMinutes(8)));
        Assert.Equal(RefreshOutcome.Invalid, session.Refresh("t0", "x", Now.AddMinutes(9), Lifetime));
        Assert.Equal(RefreshOutcome.Reused, session.Refresh("t7", "y", Now.AddMinutes(9), Lifetime));
    }
}
