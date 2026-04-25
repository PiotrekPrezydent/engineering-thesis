using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.Modules.Communication.Domain.Clients.Events
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