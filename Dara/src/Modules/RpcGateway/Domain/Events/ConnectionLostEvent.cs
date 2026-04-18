
using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.Modules.RpcGateway.Domain.Events;

public class ConnectionLostEvent : IDomainEvent
{
    public Connection LostConnection { get; }

    public ConnectionLostEvent(Connection lostConnection)
    {
        LostConnection = lostConnection;
    }
}