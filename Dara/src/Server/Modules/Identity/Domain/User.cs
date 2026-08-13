using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Events;

namespace Dara.Server.Modules.Identity.Domain;

//core client representation that is used to represent same client in other modules
public class User : Entity, IAggregateRoot
{
    public UserId Id { get; private set; }

    public string Identifier { get; private set; }

    private User() { }

    internal User(string identifier)
    {
        Id = new UserId(Guid.NewGuid());
        Identifier = identifier;
        
        AddDomainEvent(new NewUserCreatedDomainEvent(Id));
    }

    public static User Create(string identifier)
    {
        return new(identifier);
    }
    
    public bool IsIdentifiedBy(string identifier)
    {
        return Identifier == identifier;
    }
}