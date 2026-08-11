using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Profiles.Domain.Events;

namespace Dara.Server.Modules.Profiles.Domain;

public class Profile : Entity, IAggregateRoot, IHasSnapshot<ProfileSnapshot>
{
    public ProfileId ProfileId { get; private set; }

    private string _name;

    private Profile() { }

    internal Profile(ProfileId profileId, string name)
    {
        ProfileId = profileId;
        _name = name;
        AddDomainEvent(new NewProfileCreatedDomainEvent(profileId));
    }
    
    public static Profile Create(ProfileId id, string name)
    {
        return new Profile(id, name);
    }

    public void UpdateName(string name)
    {
        _name = name;
        AddDomainEvent(new ProfileNameChangedDomainEvent(ProfileId, name));
    }

    public ProfileSnapshot GetSnapshot()
    {
        return new ProfileSnapshot(ProfileId, _name);
    }
}