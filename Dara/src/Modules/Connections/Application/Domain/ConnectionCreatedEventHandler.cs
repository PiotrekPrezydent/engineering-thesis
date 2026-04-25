using Dara.BuildingBlocks.Application.Domain;
using Dara.BuildingBlocks.Infrastructure.Integration;
using Dara.Modules.Connections.Domain.Connections.Events;
using Dara.Modules.Connections.Integration;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ConnectionCreatedEventHandler : IDomainEventHandler<ConnectionCreatedDomainEvent>
    {
        public ConnectionCreatedEventHandler()
        {
        }
    
        public async Task HandleAsync(ConnectionCreatedDomainEvent domainEvent)
        {
            await IntegrationEventBus.Instance.Publish(
                new ConnectionCreatedIntegrationEvent(domainEvent.ConnectionId.Value));
        }
    }
}