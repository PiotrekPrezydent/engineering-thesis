using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Connections.Events;

public class ConnectionCreatedEvent : BaseDomainEvent
{
    public Connection CreatedConnection { get; }

    public ConnectionCreatedEvent(Connection createdConnection)
    {
        CreatedConnection = createdConnection;
    }
}