using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.Modules.Communication.Domain.Clients.Events;

public class ClientNameChangedDomainEvent : IDomainEvent
{
    public ClientId ClientId { get; }
    
    public ClientName NewName { get; }
    
    public ClientNameChangedDomainEvent(ClientId clientId, ClientName newName)
    {
        ClientId = clientId;
        NewName = newName;
    }
}