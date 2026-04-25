using Dara.BuildingBlocks.Application.Domain;
using Dara.Modules.Connections.Domain.Connections.Events;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ConnectionCreatedEventHandler : IDomainEventHandler<ConnectionCreatedDomainEvent>
    {
        public ConnectionCreatedEventHandler()
        {
        }
    
        public async Task HandleAsync(ConnectionCreatedDomainEvent domainEvent)
        {
        }
    }
}