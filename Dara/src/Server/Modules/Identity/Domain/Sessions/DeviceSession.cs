using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Rules;

namespace Dara.Server.Modules.Identity.Domain.Sessions;

// the session of one device: one row per device, created before its first sign-in (the server gives the id),
// the device belongs to one account for good; it signs in only with a one-time code from the account panel
public class DeviceSession : SessionBase, IAggregateRoot
{
    public DeviceId Id { get; private set; }
    // the device was taken away (devices module); it gets no code and cannot sign in again
    public DateTime? RevokedAt { get; private set; }
    // the open code of the panel, null when none was issued or it was used
    public DeviceLoginCode? LoginCode { get; private set; }

    private DeviceSession() { }

    private DeviceSession(DeviceId id, UserId userId, DateTime now) : base(userId, now)
    {
        Id = id;
    }

    public bool IsRevoked => RevokedAt != null;

    // no tokens yet: the device signs in later with a code
    public static DeviceSession Register(Guid deviceId, UserId userId, DateTime now)
    {
        var session = new DeviceSession(new DeviceId(deviceId), userId, now);
        session.CheckRule(new DeviceSessionStartRequiresDeviceIdRule(deviceId));
        return session;
    }

    // a new code replaces the previous one
    public void IssueLoginCode(string codeHash, DateTime now, TimeSpan lifetime)
    {
        CheckRule(new DeviceMustNotBeRevokedRule(IsRevoked));

        LoginCode = new DeviceLoginCode(codeHash, now, now + lifetime);
        Changed();
    }

    // the code is used once; a sign-in ends the tokens the device had before (one installation per device)
    // and opens a signed-out session again
    public DeviceSignInOutcome SignIn(string codeHash, string tokenHash, DateTime now, TimeSpan lifetime)
    {
        if (LoginCode == null || LoginCode.Hash != codeHash || LoginCode.IsExpired(now))
            return DeviceSignInOutcome.InvalidLoginCode;

        LoginCode = null;
        if (IsRevoked)
        {
            Changed();
            return DeviceSignInOutcome.DeviceRevoked;
        }

        RevokeActiveTokens(now);
        EndedAt = null;
        IssueToken(tokenHash, now, lifetime);
        return DeviceSignInOutcome.SignedIn;
    }

    // a repeated revocation changes nothing
    public void Revoke(DateTime now)
    {
        if (IsRevoked)
            return;

        RevokedAt = now;
        LoginCode = null;
        SignOut(now);
        Changed();
    }
}
