using Dara.BuildingBlocks.Application.Domain;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ClientNameChangedEventHandler : IDomainEventHandler<ClientNameChangedDomainEvent>
    {
        public ClientNameChangedEventHandler()
        {
        }
    
        public async Task HandleAsync(ClientNameChangedDomainEvent domainEvent)
        {
        }
    }
}