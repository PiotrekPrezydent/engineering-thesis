using Dara.BuildingBlocks.Application.Domain;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Domain
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