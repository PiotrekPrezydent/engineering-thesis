using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Clients.Events;

public class ClientRemovedEvent : BaseDomainEvent
{
    public Client RemovedClient { get; }
    
    public ClientRemovedEvent(Client removedClient)
    {
        RemovedClient = removedClient;
    }
}