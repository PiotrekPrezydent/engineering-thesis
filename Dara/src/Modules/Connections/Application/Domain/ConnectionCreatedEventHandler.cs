using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Connections.Events;
using Dara.Modules.Connections.Integration;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ConnectionCreatedEventHandler : IHandler<ConnectionCreatedDomainEvent>
    {
        public ConnectionCreatedEventHandler()
        {
        }
    
        public async Task HandleAsync(ConnectionCreatedDomainEvent domainEvent)
        {
        }
    }
}