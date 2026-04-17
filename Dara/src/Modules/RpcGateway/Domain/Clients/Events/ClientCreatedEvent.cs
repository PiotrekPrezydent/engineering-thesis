using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Clients.Events;

public class ClientCreatedEvent : BaseDomainEvent
{
    public Client CreatedClient { get; }
    
    public ClientCreatedEvent(Client createdClient)
    {
        CreatedClient = createdClient;
    }
}