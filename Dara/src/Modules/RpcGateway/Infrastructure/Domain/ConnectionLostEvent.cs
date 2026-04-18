using Dara.BuildingBlocks.Infrastructure;
using Dara.Modules.RpcGateway.Domain.Events;

namespace Dara.Modules.RpcGateway.Infrastructure.Domain;

public class ConnectionLostEventHandler : IDomainEventHandler<ConnectionLostEvent>
{
    public ConnectionLostEventHandler()
    {
        
    }
    //sync other modules
    public async Task HandleAsync(ConnectionLostEvent domainEvent)
    {
        
    }
}