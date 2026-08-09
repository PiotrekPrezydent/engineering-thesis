using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Events;

namespace Dara.Server.Modules.Identity.Domain;

//core user representation that is used to create and maintain other 
public class Client : Entity, IAggregateRoot
{
    public ClientId ClientId { get; private set; }
    
    public string ClientIdentifier { get; private set; }

    private Client() { }

    internal Client(string clientIdentifier)
    {
        ClientId = new ClientId(Guid.NewGuid());
        ClientIdentifier = clientIdentifier;
        
        AddDomainEvent(new NewClientCreatedDomainEvent(ClientId));
    }

    public static Client Create(string identifier)
    {
        return new(identifier);
    }
}