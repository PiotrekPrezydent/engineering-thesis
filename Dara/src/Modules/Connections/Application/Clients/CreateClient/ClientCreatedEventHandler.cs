using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Clients.CreateClient
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