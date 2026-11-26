namespace Dara.Server.Modules.Identity.Domain.Sessions;

// the one-time code the account panel shows for a device; only its hash is stored, the code goes to the panel once;
// a device has at most one open code, so a new code replaces the previous one; a class, not a record: ef tracks
// owned instances by reference
public class DeviceLoginCode
{
    public string Hash { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private DeviceLoginCode() { }

    internal DeviceLoginCode(string hash, DateTime issuedAt, DateTime expiresAt)
    {
        Hash = hash;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired(DateTime now)
    {
        return ExpiresAt <= now;
    }
}
