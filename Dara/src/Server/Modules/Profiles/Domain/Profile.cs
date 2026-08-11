using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Profiles.Domain.Events;

namespace Dara.Server.Modules.Profiles.Domain;

public class Profile : Entity, IAggregateRoot, IHasSnapshot<ProfileSnapshot>
{
    public ProfileId Id { get; private set; }

    private string _name;

    private Profile() { }

    internal Profile(ProfileId id, string name)
    {
        Id = id;
        _name = name;
        AddDomainEvent(new ProfileCreatedDomainEvent(id));
    }
    
    public static Profile Create(ProfileId id, string name)
    {
        return new Profile(id, name);
    }

    public void UpdateName(string name)
    {
        _name = name;
        AddDomainEvent(new ProfileNameChangedDomainEvent(Id, name));
    }

    public ProfileSnapshot GetSnapshot()
    {
        return new ProfileSnapshot(Id, _name);
    }
}