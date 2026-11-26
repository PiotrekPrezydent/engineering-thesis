using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.UnitTests;

// the session of the panel cookie: valid until a sign-out or 30 days without use
public class AccountSessionTests
{
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan IdleLifetime = TimeSpan.FromDays(30);
    private static readonly UserId UserId = new(Guid.NewGuid());

    [Fact]
    public void EveryUseMovesTheIdleLifetimeOn()
    {
        var session = AccountSession.Start(UserId, Now);

        Assert.True(session.Use(Now.AddDays(29), IdleLifetime));
        Assert.True(session.Use(Now.AddDays(58), IdleLifetime));
        Assert.Equal(Now.AddDays(58), session.LastUsedAt);
    }

    [Fact]
    public void ASessionNotUsedForThirtyDaysIsNotValid()
    {
        var session = AccountSession.Start(UserId, Now);

        Assert.False(session.Use(Now + IdleLifetime, IdleLifetime));
        Assert.Equal(Now, session.LastUsedAt);
    }

    [Fact]
    public void ASignedOutSessionIsNotValidAndARepeatedSignOutChangesNothing()
    {
        var session = AccountSession.Start(UserId, Now);

        session.SignOut(Now);
        session.SignOut(Now.AddMinutes(1));

        Assert.Equal(Now, session.EndedAt);
        Assert.False(session.Use(Now.AddMinutes(2), IdleLifetime));
    }
}
