using Dara.BuildingBlocks.Infrastructure.Events;
using Dara.Modules.Communication.Domain.Connections;
using Dara.Modules.Communication.Domain.Connections.Events;

namespace Dara.Modules.Communication.Infrastructure
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
            foreach (var domainEvent in aggregateRoot.DomainEvents)
                await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
            aggregateRoot.ClearDomainEvents();
        
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
            
                await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
            }
        
            aggregateRoot.ClearDomainEvents();
        
            if (isDeleted)
                return;
        
            int index = _connections.IndexOf(aggregateRoot);
            _connections[index] = aggregateRoot;
        }
    }
}