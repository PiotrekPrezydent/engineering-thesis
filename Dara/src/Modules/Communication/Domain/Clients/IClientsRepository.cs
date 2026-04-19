using Dara.BuildingBlocks.Domain;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Domain.Clients;

public interface IClientsRepository : IRepository<Client>
{
    public Task<Client> GetByClientConnectionIdAsync(ClientConnectionId clientConnectionId);
    
    public Task<Client> GetByClientNode(Node node);
    
    public Task<IEnumerable<Client>> GetAllWithClientConnectionIpAsync(ClientConnectionIp clientConnectionIp);
}