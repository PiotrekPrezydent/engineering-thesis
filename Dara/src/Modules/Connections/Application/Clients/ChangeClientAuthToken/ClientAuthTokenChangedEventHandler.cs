using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientAuthToken
{
    public class ClientAuthTokenChangedEventHandler : IDomainEventHandler<ClientAuthTokenChangedDomainEvent>
    {
        public ClientAuthTokenChangedEventHandler()
        {
        }
    
        public async Task HandleAsync(ClientAuthTokenChangedDomainEvent domainEvent)
        {
        }
    }
}