using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Clients.Events;

public class ClientNameChangedEvent : BaseDomainEvent
{
    public Client ChangedClient { get; }

    public ClientNameChangedEvent(Client changedClient)
    {
        ChangedClient = changedClient;
    }
}