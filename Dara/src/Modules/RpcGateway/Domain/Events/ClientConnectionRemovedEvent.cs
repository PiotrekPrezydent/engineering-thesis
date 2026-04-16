using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Events;

public class ClientConnectionRemovedEvent : BaseDomainEvent
{
    public ClientConnection CreatedClientConnection { get; }

    public ClientConnectionRemovedEvent(ClientConnection createdClientConnection)
    {
        CreatedClientConnection = createdClientConnection;
    }
}