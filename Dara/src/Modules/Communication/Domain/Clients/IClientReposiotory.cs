using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.Clients;

public interface IClientReposiotory : IRepository<Client>
{
    public Task<Client> GetByClientConnectionAsync(ClientConnection clientConnection);
    
    public Task<Client> GetByNameAsync(ClientName clientName);
    
    public Task SaveAsync(Client client);
}