using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Clients.Application.Clients.GetClient;

public record GetClientQuery(Guid ClientId) : IQuery<ClientDto>;