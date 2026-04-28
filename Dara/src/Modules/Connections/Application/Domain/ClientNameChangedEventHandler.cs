using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ClientNameChangedEventHandler : IHandler<ClientNameChangedDomainEvent>
    {
        public ClientNameChangedEventHandler()
        {
        }
    
        public async Task HandleAsync(ClientNameChangedDomainEvent domainEvent)
        {
        }
    }
}