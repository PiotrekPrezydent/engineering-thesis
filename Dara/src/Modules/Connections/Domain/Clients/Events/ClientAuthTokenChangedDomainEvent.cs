using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.Connections.Domain.Clients.Events
{
    public class ClientAuthTokenChangedDomainEvent : IDomainEvent
    {
        public ClientId ClientId { get; }
    
        public ClientAuthToken NewAuthToken { get; }
    
        public ClientAuthTokenChangedDomainEvent(ClientId clientId, ClientAuthToken newAuthToken)
        {
            ClientId = clientId;
            NewAuthToken = newAuthToken;
        }
    
    }
}