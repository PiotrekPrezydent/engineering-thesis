using Dara.BuildingBlocks.Infrastructure.Domain;
using Dara.Modules.Connections.Domain.Clients;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Infrastructure
{
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
            return _clients.FirstOrDefault(c => c.Id.Value == id.Value);
        }

        public async Task <IEnumerable<Client>> GetAllAsync()
        {
            return _clients.AsEnumerable();
        }

        public async Task AddAsync(Client aggregateRoot)
        {
            foreach (var domainEvent in aggregateRoot.DomainEvents)
                await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
            aggregateRoot.ClearDomainEvents();
        
            _clients.Add(aggregateRoot);
        }

        public async Task RemoveAsync(Client aggregateRoot)
        {
            _clients.Remove(aggregateRoot);
        }

        public async Task SaveAsync(Client aggregateRoot)
        {
            bool isDeleted = false;
        
            foreach (var domainEvent in aggregateRoot.DomainEvents)
            {
                if ((dynamic)domainEvent is ClientDeletedDomainEvent)
                    isDeleted = true;
            
                await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
            }
        
            aggregateRoot.ClearDomainEvents();
        
            if (isDeleted)
                return;
        
            int index = _clients.IndexOf(aggregateRoot);
            _clients[index] = aggregateRoot;
        }
    }
}