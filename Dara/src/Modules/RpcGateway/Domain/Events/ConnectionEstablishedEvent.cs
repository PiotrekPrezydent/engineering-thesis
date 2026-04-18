using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.Modules.RpcGateway.Domain.Events;

public class ConnectionEstablishedEvent : IDomainEvent
{
    public Connection CreatedConnection { get; }

    public ConnectionEstablishedEvent(Connection createdConnection)
    {
        CreatedConnection = createdConnection;
    }
}