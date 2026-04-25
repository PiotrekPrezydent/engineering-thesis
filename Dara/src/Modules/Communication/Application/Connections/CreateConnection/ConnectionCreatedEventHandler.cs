using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Connections.Events;

namespace Dara.Modules.Communication.Application.Connections.CreateConnection;

public class ConnectionCreatedEventHandler : IDomainEventHandler<ConnectionCreatedDomainEvent>
{
    public ConnectionCreatedEventHandler()
    {
    }
    
    public async Task HandleAsync(ConnectionCreatedDomainEvent domainEvent)
    {
    }
}