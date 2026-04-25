using Dara.BuildingBlocks.Application.Domain;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ClientCreatedEventHandler : IDomainEventHandler<ClientCreatedDomainEvent>
    {
        public ClientCreatedEventHandler()
        {
        }
    
        public async Task HandleAsync(ClientCreatedDomainEvent domainEvent)
        {
        }
    }
}