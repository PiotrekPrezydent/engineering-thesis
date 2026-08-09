using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Profiles.Domain.Events;

namespace Dara.Server.Modules.Profiles.Domain;

public class Profile : Entity, IAggregateRoot
{
    public ProfileId ProfileId { get; private set; }
    
    public string Name { get; private set; }

    private Profile() { }

    internal Profile(ProfileId profileId, string name)
    {
        ProfileId = profileId;
        Name = name;
        AddDomainEvent(new ProfileCreatedDomainEvent(profileId));
    }
    
    public static Profile Create(ProfileId id, string name)
    {
        return new Profile(id, name);
    }

    public void UpdateName(string name)
    {
        Name = name;
        AddDomainEvent(new ProfileNameChangedDomainEvent(ProfileId, name));
    }
}