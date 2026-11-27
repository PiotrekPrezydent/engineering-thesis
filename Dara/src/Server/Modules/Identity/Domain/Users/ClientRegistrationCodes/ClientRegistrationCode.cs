using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users.ClientRegistrationCodes;

// the one-time code the account panel shows for a new client; the app of the client registers itself with it and
// gets the id of the client and its tokens; only the hash is stored
public class ClientRegistrationCode : Entity
{
    public ClientRegistrationCodeId Id { get; private set; }
    public string Hash { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private ClientRegistrationCode() { }

    private ClientRegistrationCode(string hash, DateTime issuedAt, DateTime expiresAt)
    {
        Id = new ClientRegistrationCodeId(Guid.NewGuid());
        Hash = hash;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    internal static ClientRegistrationCode Create(string hash, DateTime now, TimeSpan lifetime)
    {
        return new ClientRegistrationCode(hash, now, now + lifetime);
    }

    public bool IsExpired(DateTime now)
    {
        return ExpiresAt <= now;
    }
}
