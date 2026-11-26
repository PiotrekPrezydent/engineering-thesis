namespace Dara.Server.Modules.Identity.Domain.Sessions;

// one refresh token of a session; only the hash of the token is stored, the token itself goes to the client once
public class RefreshToken
{
    public Guid Id { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    // set when the token was exchanged for a new one
    public DateTime? ConsumedAt { get; private set; }
    // set on sign-out, on a new sign-in of the device, on revocation and when a used token comes back
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken() { }

    internal RefreshToken(string tokenHash, DateTime issuedAt, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        TokenHash = tokenHash;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    public bool IsActive(DateTime now)
    {
        return ConsumedAt == null && RevokedAt == null && ExpiresAt > now;
    }

    public bool IsExpired(DateTime now)
    {
        return ExpiresAt <= now;
    }

    internal void Consume(DateTime now)
    {
        ConsumedAt = now;
    }

    internal void Revoke(DateTime now)
    {
        RevokedAt ??= now;
    }
}
