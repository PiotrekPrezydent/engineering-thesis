using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Profiles.Domain.Events;

namespace Dara.Server.Modules.Profiles.Domain;

public class Profile : Entity, IAggregateRoot
{
    public ProfileId Id { get; private set; }
    
    public string Name { get; private set; }
    internal Profile(ProfileId id, string name)
    {
        Id = id;
        Name = name;
        AddDomainEvent(new ProfileCreatedDomainEvent(id));
    }


    public static Profile Create(ProfileId id, string name)
    {
        return new Profile(id, name);
    }

    public void UpdateName(string name)
    {
        Name = name;
        AddDomainEvent(new ProfileNameChangedDomainEvent(Id, name));
    }
    
}