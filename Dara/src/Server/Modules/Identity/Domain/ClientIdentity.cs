using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Events;

namespace Dara.Server.Modules.Identity.Domain;

//core client representation that is used to represent same client in other modules
public class ClientIdentity : Entity, IAggregateRoot
{
    public ClientIdentityId ClientId { get; private set; }

    private string _identifier;

    private ClientIdentity() { }

    internal ClientIdentity(string identifier)
    {
        ClientId = new ClientIdentityId(Guid.NewGuid());
        _identifier = identifier;
        
        AddDomainEvent(new NewClientIdentityCreatedDomainEvent(ClientId));
    }

    public static ClientIdentity Create(string identifier)
    {
        return new(identifier);
    }

    public bool IsIdentifiedBy(string identifier)
    {
        return _identifier == identifier;
    }
}