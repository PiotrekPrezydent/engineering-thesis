using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.Connections.Domain.Connections.Events
{
    public class ConnectionDeletedDomainEvent : IDomainEvent
    {
        public ConnectionId ConnectionId { get; }
    
        public ConnectionDeletedDomainEvent(ConnectionId connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}