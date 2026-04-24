using Dara.BuildingBlocks.Infrastructure.Events;
using Dara.Modules.Communication.Domain.Connections;

namespace Dara.Modules.Communication.Infrastructure;

public class ConnectionRepository : IConnectionRepository
{
    private readonly List<Connection> _connections;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ConnectionRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _connections = new();
    }
    
    public async Task<Connection> GetByIdAsync(ConnectionId id)
    {
        return _connections.FirstOrDefault(c => c.Id == id);
    }

    public async Task<IEnumerable<Connection>> GetAllAsync()
    {
        return _connections.AsEnumerable();
    }

    public async Task AddAsync(Connection aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        _connections.Add(aggregateRoot);
    }

    public async Task RemoveAsync(Connection aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        _connections.Remove(aggregateRoot);
    }

    public async Task SaveAsync(Connection aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        int index = _connections.IndexOf(aggregateRoot);
        _connections[index] = aggregateRoot;
    }
}