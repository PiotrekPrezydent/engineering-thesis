namespace Dara.Server.Modules.Identity.Application.Clients;

// a client as the panel of its account and the client itself see it
public record UserClientDto(
    Guid ClientId,
    Guid UserId,
    string Name,
    string ClientType,
    bool IsRevoked,
    DateTime RegisteredAt,
    DateTime? RevokedAt,
    IReadOnlyDictionary<string, string> Context);
