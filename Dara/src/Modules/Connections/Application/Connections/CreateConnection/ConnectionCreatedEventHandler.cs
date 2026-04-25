using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Connections.Domain.Connections.Events;

namespace Dara.Modules.Connections.Application.Connections.CreateConnection
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