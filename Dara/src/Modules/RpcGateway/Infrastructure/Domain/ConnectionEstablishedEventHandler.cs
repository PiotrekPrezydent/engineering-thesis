using Dara.BuildingBlocks.Infrastructure;
using Dara.Modules.RpcGateway.Domain.Events;

namespace Dara.Modules.RpcGateway.Infrastructure.Domain;

public class ConnectionEstablishedEventHandler : IDomainEventHandler<ConnectionEstablishedEvent>
{
    public ConnectionEstablishedEventHandler()
    {
        
    }
    //sync other modules
    public async Task HandleAsync(ConnectionEstablishedEvent domainEvent)
    {
        
    }
}