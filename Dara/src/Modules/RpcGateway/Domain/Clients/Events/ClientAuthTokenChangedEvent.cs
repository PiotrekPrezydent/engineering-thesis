using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Clients.Events;

public class ClientAuthTokenChangedEvent : BaseDomainEvent
{
    public Client ChangedClient { get; }

    public ClientAuthTokenChangedEvent(Client changedClient)
    {
        ChangedClient = changedClient;
    }
}