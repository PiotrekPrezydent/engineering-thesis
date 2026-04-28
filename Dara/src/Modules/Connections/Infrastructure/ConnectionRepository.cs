using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.Modules.Connections.Domain.Connections;
using Dara.Modules.Connections.Domain.Connections.Events;

namespace Dara.Modules.Connections.Infrastructure
{
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
            return _connections.FirstOrDefault(c => c.Id.Value == id.Value);
        }

        public async Task<IEnumerable<Connection>> GetAllAsync()
        {
            return _connections.AsEnumerable();
        }

        public async Task AddAsync(Connection aggregateRoot)
        {
            await _domainEventDispatcher.DispatchEntityEventsAsync(aggregateRoot);
        
            _connections.Add(aggregateRoot);
        }

        public async Task RemoveAsync(Connection aggregateRoot)
        {
            _connections.Remove(aggregateRoot);
        }

        public async Task SaveAsync(Connection aggregateRoot)
        {
            bool isDeleted = false;
        
            foreach (var domainEvent in aggregateRoot.DomainEvents)
            {
                if ((dynamic)domainEvent is ConnectionDeletedDomainEvent)
                    isDeleted = true;
            }
            
            await _domainEventDispatcher.DispatchEntityEventsAsync(aggregateRoot);
        
            if (isDeleted)
                return;
        
            int index = _connections.IndexOf(aggregateRoot);
            _connections[index] = aggregateRoot;
        }
    }
}