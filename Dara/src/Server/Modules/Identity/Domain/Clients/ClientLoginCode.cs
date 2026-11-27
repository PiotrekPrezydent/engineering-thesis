using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients;

// the one-time code the account panel shows for an existing client; only its hash is stored, the code goes to the
// panel once; a client has at most one open code
public class ClientLoginCode : Entity
{
    public ClientLoginCodeId Id { get; private set; }
    public string Hash { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private ClientLoginCode() { }

    private ClientLoginCode(string hash, DateTime issuedAt, DateTime expiresAt)
    {
        Id = new ClientLoginCodeId(Guid.NewGuid());
        Hash = hash;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    internal static ClientLoginCode Create(string hash, DateTime now, TimeSpan lifetime)
    {
        return new ClientLoginCode(hash, now, now + lifetime);
    }

    public bool IsExpired(DateTime now)
    {
        return ExpiresAt <= now;
    }
}
