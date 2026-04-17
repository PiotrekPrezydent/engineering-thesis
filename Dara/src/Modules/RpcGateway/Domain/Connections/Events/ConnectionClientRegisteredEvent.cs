using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Connections.Events;

public class ConnectionClientRegisteredEvent : BaseDomainEvent
{
    public Connection ChangedConnection { get; }
    
    public ConnectionClientRegisteredEvent(Connection changedConnection)
    {
        ChangedConnection = changedConnection;
    }
}