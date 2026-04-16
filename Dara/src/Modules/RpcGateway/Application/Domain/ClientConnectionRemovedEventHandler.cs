using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.RpcGateway.Domain.Events;

namespace Dara.Modules.RpcGateway.Application.Domain;

public class ClientConnectionRemovedEventHandler : IDomainEventHandler<ClientConnectionRemovedEvent>
{
    public Task HandleAsync(ClientConnectionRemovedEvent domainEvent)
    {
        throw new NotImplementedException();
    }
}