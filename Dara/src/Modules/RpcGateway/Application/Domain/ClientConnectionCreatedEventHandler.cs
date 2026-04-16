using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.RpcGateway.Domain.Events;

namespace Dara.Modules.RpcGateway.Application.Domain;

public class ClientConnectionCreatedEventHandler : IDomainEventHandler<ClientConnectionCreatedEvent>
{
    public async Task HandleAsync(ClientConnectionCreatedEvent domainEvent)
    {
        Console.WriteLine("CLIENT CREATED");
    }
}