using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.Connections.Domain.Connections.Events
{
    public class ConnectionCreatedDomainEvent : IDomainEvent
    {
        public ConnectionId ConnectionId { get; }
    
        public ConnectionCreatedDomainEvent(ConnectionId connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}