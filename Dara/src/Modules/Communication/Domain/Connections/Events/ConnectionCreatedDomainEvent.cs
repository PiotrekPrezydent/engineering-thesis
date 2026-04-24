using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.Modules.Communication.Domain.Connections.Events;

public class ConnectionCreatedDomainEvent : IDomainEvent
{
    public ConnectionId ConnectionId { get; }
    
    public ConnectionCreatedDomainEvent(ConnectionId connectionId)
    {
        ConnectionId = connectionId;
    }
}