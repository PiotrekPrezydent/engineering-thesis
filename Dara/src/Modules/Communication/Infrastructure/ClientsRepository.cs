using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.BuildingBlocks.Domain.Exceptions;
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
    
    public async Task<Client> GetByGuidAsync(Guid guid)
    {
        var client = _clients.FirstOrDefault(e => e.Id == guid, null);
        
        if(client == null)
            throw new EntityNotFoundInRepositoryException<ClientsRepository,Client>(this, guid);
        
        return client;
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return _clients.AsEnumerable();
    }

    public async Task AddAsync(Client aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);

        _clients.Add(aggregateRoot);
    }

    public async Task RemoveAsync(Client aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        await _domainEventDispatcher.DispatchAsync(new EntityRemovedEvent<Client>(this, aggregateRoot));
        
        _clients.Remove(aggregateRoot);
    }

    public async Task<Client> GetByClientConnectionIdAsync(ClientConnectionId clientConnectionId)
    {
        var client = _clients.FirstOrDefault(e => e.ClientConnectionId == clientConnectionId, null);
        
        if(client == null)
            throw new EntityNotFoundInRepositoryException<ClientsRepository,Client>(this, clientConnectionId);
        
        return client;
    }

    public async Task<Client> GetByClientNode(Node node)
    {
        var client = _clients.FirstOrDefault(e => e.ClientNode == node, null);
        
        if(client == null)
            throw new EntityNotFoundInRepositoryException<ClientsRepository,Client>(this, node);
        
        return client;
    }

    public async Task<IEnumerable<Client>> GetAllWithClientConnectionIpAsync(ClientConnectionIp clientConnectionIp)
    {
        return _clients.Where(e => e.ClientConnectionIp == clientConnectionIp).AsEnumerable();
    }
}