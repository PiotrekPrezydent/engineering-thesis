using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Profiles.Domain.Events;

namespace Dara.Server.Modules.Profiles.Domain;

public class Profile : Entity, IAggregateRoot
{
    public ProfileId Id { get; private set; }

    public string Name { get; private set; }

    private Profile() { }

    internal Profile(ProfileId id, string name)
    {
        Id = id;
        Name = name;
        AddDomainEvent(new ProfileCreatedDomainEvent(id));
    }
    
    public static Profile CreateDefault(ProfileId id)
    {
        return new Profile(id, "DEFAULT-NAME");
    }

    public void UpdateName(string name)
    {
        Name = name;
        AddDomainEvent(new ProfileNameChangedDomainEvent(Id, name));
    }
}