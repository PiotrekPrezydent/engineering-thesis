using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients.Events;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ClientAuthTokenChangedEventHandler : IHandler<ClientAuthTokenChangedDomainEvent>
    {
        public ClientAuthTokenChangedEventHandler()
        {
        }
    
        public async Task HandleAsync(ClientAuthTokenChangedDomainEvent domainEvent)
        {
        }
    }
}