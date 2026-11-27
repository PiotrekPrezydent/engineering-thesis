using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.UserSessions;

namespace Dara.Server.Modules.Identity.UnitTests;

// the sign-ins of browsers in the session of the panel client: each valid until a sign-out or 30 days without use
public class PanelSignInTests
{
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan IdleLifetime = TimeSpan.FromDays(30);
    private static readonly UserId UserId = new(Guid.NewGuid());

    private static UserSession NewPanelSession()
    {
        return UserSession.StartPanel(new ClientId(Guid.NewGuid()), UserId, Now);
    }

    [Fact]
    public void EveryUseMovesTheIdleLifetimeOn()
    {
        var session = NewPanelSession();
        var signIn = session.StartPanelSignIn(Now, IdleLifetime);

        Assert.True(session.UsePanelSignIn(signIn.Id, Now.AddDays(29), IdleLifetime));
        Assert.True(session.UsePanelSignIn(signIn.Id, Now.AddDays(58), IdleLifetime));
        Assert.Equal(Now.AddDays(58), signIn.LastUsedAt);
    }

    [Fact]
    public void ASignInNotUsedForThirtyDaysIsNotValid()
    {
        var session = NewPanelSession();
        var signIn = session.StartPanelSignIn(Now, IdleLifetime);

        Assert.False(session.UsePanelSignIn(signIn.Id, Now + IdleLifetime, IdleLifetime));
        Assert.Equal(Now, signIn.LastUsedAt);
    }

    [Fact]
    public void ASignedOutSignInIsNotValidAndARepeatedSignOutChangesNothing()
    {
        var session = NewPanelSession();
        var signIn = session.StartPanelSignIn(Now, IdleLifetime);

        session.SignOutPanelSignIn(signIn.Id, Now);
        session.SignOutPanelSignIn(signIn.Id, Now.AddMinutes(1));

        Assert.Equal(Now, signIn.EndedAt);
        Assert.False(session.UsePanelSignIn(signIn.Id, Now.AddMinutes(2), IdleLifetime));
    }

    // several browsers act as one panel client; a sign-out of one leaves the others
    [Fact]
    public void BrowsersSignInApartAndOldSignInsGoWithTheNextOne()
    {
        var session = NewPanelSession();
        var first = session.StartPanelSignIn(Now, IdleLifetime);
        var second = session.StartPanelSignIn(Now, IdleLifetime);

        session.SignOutPanelSignIn(first.Id, Now.AddMinutes(1));

        Assert.True(session.UsePanelSignIn(second.Id, Now.AddMinutes(2), IdleLifetime));
        var third = session.StartPanelSignIn(Now.AddMinutes(3), IdleLifetime);
        Assert.Equal([second.Id, third.Id], session.PanelSignIns.Select(s => s.Id));
    }

    // the sign-ins of the panel never raise the concurrency token of the session, so parallel requests do not conflict
    [Fact]
    public void SignInsDoNotChangeTheVersion()
    {
        var session = NewPanelSession();
        var version = session.Version;

        var signIn = session.StartPanelSignIn(Now, IdleLifetime);
        session.UsePanelSignIn(signIn.Id, Now.AddMinutes(1), IdleLifetime);
        session.SignOutPanelSignIn(signIn.Id, Now.AddMinutes(2));

        Assert.Equal(version, session.Version);
    }

    [Fact]
    public void AnUnknownSignInIsNotValid()
    {
        var session = NewPanelSession();

        Assert.False(session.UsePanelSignIn(new PanelSignInId(Guid.NewGuid()), Now, IdleLifetime));
    }
}
