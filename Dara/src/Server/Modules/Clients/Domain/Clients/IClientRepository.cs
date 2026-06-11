using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients;

public interface IClientRepository : IRepository
{
    Task AddAsync(Client client);
    Task DeleteAsync(Client client);
    Task<Client> GetByClientByIdAsync(ClientId clientId);
}