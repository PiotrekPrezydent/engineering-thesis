using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.Modules.Communication.Domain.Connections.Events
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