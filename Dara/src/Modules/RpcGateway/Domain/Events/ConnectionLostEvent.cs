using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain.Events;

public class ConnectionLostEvent : BaseDomainEvent
{
    public Connection LostConnection { get; }

    public ConnectionLostEvent(Connection lostConnection)
    {
        LostConnection = lostConnection;
    }
}