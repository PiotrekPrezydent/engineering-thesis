using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Events;

public class ClientConnectionCreatedEvent : BaseDomainEvent
{
    public ClientConnection CreatedClientConnection { get; }

    public ClientConnectionCreatedEvent(ClientConnection createdClientConnection)
    {
        CreatedClientConnection = createdClientConnection;
    }
}