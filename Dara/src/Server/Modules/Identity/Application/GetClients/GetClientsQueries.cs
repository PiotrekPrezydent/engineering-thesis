using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.Modules.Identity.Application.Clients;

namespace Dara.Server.Modules.Identity.Application.GetClients;

// the panel: every client of the account, revoked ones too, oldest first
public record GetAccountClientsQuery(Guid UserId) : IQuery<List<UserClientDto>>;

// the panel: one client of the account; null for a missing client or a client of another account
public record GetAccountClientQuery(Guid UserId, Guid ClientId) : IQuery<UserClientDto?>;

// the client token: the client itself only
public record GetCurrentClientQuery(Guid ClientId) : IQuery<UserClientDto?>;

// for the host (names in notifications): null when there is no such client
public record GetClientQuery(Guid ClientId) : IQuery<ClientDto?>;

public record GetClientsQuery(IEnumerable<Guid> ClientIds) : IQuery<List<ClientDto>>;
