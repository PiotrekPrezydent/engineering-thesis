using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ClientCreatedEventHandler : IHandler<ClientCreatedDomainEvent>
    {
        public ClientCreatedEventHandler()
        {
        }
    
        public async Task HandleAsync(ClientCreatedDomainEvent domainEvent)
        {
        }
    }
}