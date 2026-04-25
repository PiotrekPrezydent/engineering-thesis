using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Connections.Domain.Clients;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Clients.DeleteClient
{
    public class ClientDeletedEventHandler : IDomainEventHandler<ClientDeletedDomainEvent>
    {
        IClientRepository _clientRepository;
    
        public ClientDeletedEventHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
    
        public async Task HandleAsync(ClientDeletedDomainEvent domainEvent)
        {
            Client client = await _clientRepository.GetByIdAsync(domainEvent.ClientId);
        
            await _clientRepository.RemoveAsync(client);
        }
    }
}