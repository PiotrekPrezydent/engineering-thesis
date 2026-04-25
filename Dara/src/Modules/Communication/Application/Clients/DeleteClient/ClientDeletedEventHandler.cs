using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Clients.Events;

namespace Dara.Modules.Communication.Application.Clients.DeleteClient;

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