namespace Dara.Server.Modules.Identity.Application.Clients;

// a client as other clients and the host see it (names in notifications): no context
public record ClientDto(Guid ClientId, Guid UserId, string Name, string ClientType, bool IsRevoked, DateTime RegisteredAt);
