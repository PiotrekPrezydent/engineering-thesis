using Dara.BuildingBlocks.Infrastructure.Abstractions;
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
            await _domainEventDispatcher.DispatchEntityEventsAsync(aggregateRoot);
            
            _clients.Add(aggregateRoot);
        }

        public async Task RemoveAsync(Client aggregateRoot)
        {
            await _domainEventDispatcher.DispatchEntityEventsAsync(aggregateRoot);
            
            _clients.Remove(aggregateRoot);
        }

        public async Task SaveAsync(Client aggregateRoot)
        {
            await _domainEventDispatcher.DispatchEntityEventsAsync(aggregateRoot);
        
            int index = _clients.IndexOf(aggregateRoot);
            _clients[index] = aggregateRoot;
        }
    }
}