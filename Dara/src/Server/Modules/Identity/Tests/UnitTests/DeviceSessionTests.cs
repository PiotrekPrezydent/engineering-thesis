using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Rules;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.UnitTests;

// a device is registered without tokens and signs in only with a one-time code of the panel
public class DeviceSessionTests
{
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan Lifetime = TimeSpan.FromDays(30);
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
    private static readonly UserId Alice = new(Guid.NewGuid());

    private static DeviceSession Register()
    {
        return DeviceSession.Register(Guid.NewGuid(), Alice, Now);
    }

    [Fact]
    public void RegisterCreatesADeviceWithoutTokensAndCode()
    {
        var deviceId = Guid.NewGuid();

        var session = DeviceSession.Register(deviceId, Alice, Now);

        Assert.Equal(deviceId, session.Id.Value);
        Assert.Equal(Alice, session.UserId);
        Assert.Empty(session.Tokens);
        Assert.Null(session.LoginCode);
        Assert.Empty(session.DomainEvents);
    }

    [Fact]
    public void RegisterRefusesAnEmptyDeviceId()
    {
        var empty = Assert.Throws<BuisnessRuleValidationException>(() => DeviceSession.Register(Guid.Empty, Alice, Now));
        Assert.IsType<DeviceSessionStartRequiresDeviceIdRule>(empty.BrokenRule);
    }

    [Fact]
    public void ACodeSignsInOnce()
    {
        var session = Register();
        session.IssueLoginCode("c1", Now, CodeLifetime);

        Assert.Equal(DeviceSignInOutcome.SignedIn, session.SignIn("c1", "t1", Now.AddMinutes(1), Lifetime));
        Assert.Null(session.LoginCode);
        Assert.Equal(DeviceSignInOutcome.InvalidLoginCode, session.SignIn("c1", "t2", Now.AddMinutes(2), Lifetime));
        Assert.Equal(RefreshOutcome.Refreshed, session.Refresh("t1", "t3", Now.AddMinutes(3), Lifetime));
    }

    [Fact]
    public void ANewCodeReplacesTheOldOne()
    {
        var session = Register();
        session.IssueLoginCode("c1", Now, CodeLifetime);
        session.IssueLoginCode("c2", Now.AddMinutes(1), CodeLifetime);

        Assert.Equal(DeviceSignInOutcome.InvalidLoginCode, session.SignIn("c1", "t1", Now.AddMinutes(2), Lifetime));
        Assert.Equal(DeviceSignInOutcome.SignedIn, session.SignIn("c2", "t1", Now.AddMinutes(2), Lifetime));
    }

    [Fact]
    public void ACodeExpiresAfterItsLifetime()
    {
        var session = Register();
        session.IssueLoginCode("c1", Now, CodeLifetime);

        Assert.Equal(DeviceSignInOutcome.InvalidLoginCode, session.SignIn("c1", "t1", Now + CodeLifetime, Lifetime));
        Assert.Empty(session.Tokens);
    }

    [Fact]
    public void ANewSignInRevokesTheTokensOfTheEarlierInstallation()
    {
        var session = Register();
        session.IssueLoginCode("c1", Now, CodeLifetime);
        session.SignIn("c1", "t1", Now, Lifetime);
        session.IssueLoginCode("c2", Now, CodeLifetime);

        Assert.Equal(DeviceSignInOutcome.SignedIn, session.SignIn("c2", "t2", Now, Lifetime));

        Assert.Equal(RefreshOutcome.Revoked, session.Refresh("t1", "x", Now, Lifetime));
        Assert.Equal(RefreshOutcome.Refreshed, session.Refresh("t2", "t3", Now, Lifetime));
    }

    [Fact]
    public void ASignInAfterSignOutOpensTheSessionAgain()
    {
        var session = Register();
        session.IssueLoginCode("c1", Now, CodeLifetime);
        session.SignIn("c1", "t1", Now, Lifetime);
        session.SignOut(Now);
        session.IssueLoginCode("c2", Now, CodeLifetime);

        Assert.Equal(DeviceSignInOutcome.SignedIn, session.SignIn("c2", "t2", Now, Lifetime));

        Assert.False(session.IsEnded);
        Assert.Equal(RefreshOutcome.Refreshed, session.Refresh("t2", "t3", Now, Lifetime));
    }

    [Fact]
    public void ARevokedDeviceLosesItsCodeAndGetsNoNewOne()
    {
        var session = Register();
        session.IssueLoginCode("c1", Now, CodeLifetime);
        session.SignIn("c1", "t1", Now, Lifetime);
        session.IssueLoginCode("c2", Now, CodeLifetime);

        session.Revoke(Now);
        var version = session.Version;
        session.Revoke(Now.AddMinutes(1));

        Assert.Equal(Now, session.RevokedAt);
        Assert.Equal(version, session.Version);
        Assert.Null(session.LoginCode);
        Assert.Equal(DeviceSignInOutcome.InvalidLoginCode, session.SignIn("c2", "t2", Now, Lifetime));
        Assert.Equal(RefreshOutcome.Revoked, session.Refresh("t1", "t3", Now, Lifetime));
        var refused = Assert.Throws<BuisnessRuleValidationException>(() => session.IssueLoginCode("c3", Now, CodeLifetime));
        Assert.IsType<DeviceMustNotBeRevokedRule>(refused.BrokenRule);
    }
}
