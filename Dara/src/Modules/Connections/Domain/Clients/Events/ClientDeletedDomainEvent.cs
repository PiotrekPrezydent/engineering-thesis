using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.Modules.Connections.Domain.Clients.Events
{
    public class ClientDeletedDomainEvent : IDomainEvent
    {
        public ClientId ClientId { get; }

        public ClientDeletedDomainEvent(ClientId clientId)
        {
            ClientId = clientId;
        }
    }
}