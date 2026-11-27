namespace Dara.Shared.Contracts.Clients;

// a client as the account panel and the client itself see it; ClientType Pc, Phone, Tablet, Watch or Other
public record AccountClientSnapshot(
    Guid ClientId,
    Guid UserId,
    string Name,
    string ClientType,
    bool IsRevoked,
    DateTime RegisteredAt,
    DateTime? RevokedAt,
    IReadOnlyDictionary<string, string> Context);
