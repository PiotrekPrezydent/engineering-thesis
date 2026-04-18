using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Events;

public class ConnectionEstablishedEvent : BaseDomainEvent
{
    public Connection CreatedConnection { get; }

    public ConnectionEstablishedEvent(Connection createdConnection)
    {
        CreatedConnection = createdConnection;
    }
}