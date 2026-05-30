using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ClientDeletedEventHandler : IHandler<ClientDeletedDomainEvent>
    {
        public ClientDeletedEventHandler()
        {
        }
    
        public async Task HandleAsync(ClientDeletedDomainEvent domainEvent)
        {
        }
    }
}