using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.Modules.Communication.Domain.Clients.Events;

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