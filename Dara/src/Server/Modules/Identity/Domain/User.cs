using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Events;

namespace Dara.Server.Modules.Identity.Domain;

//core user representation that is used to create and maintain other 
public class User : Entity, IAggregateRoot
{
    public UserId UserId { get; private set; }
    
    public string UserIdentifier { get; private set; }

    protected User()
    {
    }

    internal User(UserId userId, string userIdentifier)
    {
        UserId = userId;
        UserIdentifier = userIdentifier;
        
        AddDomainEvent(new NewUserCreatedDomainEvent(userId));
    }

    public static User CreateNewUser(string identifier)
    {
        return new(new UserId(Guid.NewGuid()), identifier);
    }
}