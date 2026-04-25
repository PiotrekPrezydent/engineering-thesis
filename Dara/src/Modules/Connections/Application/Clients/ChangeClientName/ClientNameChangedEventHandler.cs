using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientName
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