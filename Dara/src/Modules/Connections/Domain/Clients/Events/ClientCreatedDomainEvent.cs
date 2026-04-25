using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.Modules.Connections.Domain.Clients.Events
{
    public class ClientCreatedDomainEvent : IDomainEvent
    {
        public ClientId ClientId { get; }
    
        public ClientCreatedDomainEvent(ClientId clientId)
        {
            ClientId = clientId;
        }
    }
}