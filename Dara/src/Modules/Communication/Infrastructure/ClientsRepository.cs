using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Infrastructure;

public class ClientsRepository : IClientsRepository
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly List<Client> _clients;

    public ClientsRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _clients = new();
    }
    
    public Task<Client> GetByGuidAsync(Guid guid)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Client>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Client aggregateRoot)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Client aggregateRoot)
    {
        throw new NotImplementedException();
    }

    public Task<Client> GetByClientConnectionIdAsync(ClientConnectionId clientConnectionId)
    {
        throw new NotImplementedException();
    }

    public Task<Client> GetByClientNode(Node node)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Client>> GetAllWithClientConnectionIpAsync(ClientConnectionIp clientConnectionIp)
    {
        throw new NotImplementedException();
    }
}