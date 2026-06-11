using Dara.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients;

public interface IClientRepository : IRepository
{
    public Task Add(Client client);
    public Task SaveAsync(Client client);
    public Task DeleteAsync(Client client);
    public Task<Client> GetByClientIdAsync(ClientId clientId);
}