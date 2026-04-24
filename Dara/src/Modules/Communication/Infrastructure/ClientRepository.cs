using Dara.BuildingBlocks.Infrastructure.Events;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Infrastructure;

public class ClientRepository : IClientRepository
{
    private readonly List<Client> _clients;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ClientRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _clients = new();
    }
    
    public async Task <Client> GetByIdAsync(ClientId id)
    {
        return _clients.FirstOrDefault(c => c.Id == id);
    }

    public async Task <IEnumerable<Client>> GetAllAsync()
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
        
        _clients.Remove(aggregateRoot);
    }

    public async Task SaveAsync(Client aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        int index = _clients.IndexOf(aggregateRoot);
        _clients[index] = aggregateRoot;
    }
}