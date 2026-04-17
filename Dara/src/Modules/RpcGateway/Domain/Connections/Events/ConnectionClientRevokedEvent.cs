using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Connections.Events;

public class ConnectionClientRevokedEvent : BaseDomainEvent
{
    public Connection ChangedConnection { get; }
    
    public ConnectionClientRevokedEvent(Connection changedConnection)
    {
        ChangedConnection = changedConnection;
    }
}