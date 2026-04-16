using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Domain.Events;

namespace Dara.Modules.RpcGateway.Application.Domain;

public class ClientConnectionRemovedEventHandler : IDomainEventHandler<ClientConnectionRemovedEvent>
{
    public async Task HandleAsync(ClientConnectionRemovedEvent domainEvent)
    {
        Console.WriteLine("CLIENT REMOVED");
    }
}