using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Profiles.Domain.Events;

namespace Dara.Server.Modules.Profiles.Domain;

public class Profile : Entity, IAggregateRoot
{
    public ProfileId ClientProfileId { get; private set; }
    
    public string Name { get; private set; }
    internal Profile(ProfileId clientProfileId, string name)
    {
        ClientProfileId = clientProfileId;
        Name = name;
        AddDomainEvent(new ProfileCreatedDomainEvent(clientProfileId));
    }


    public static Profile Create(ProfileId id, string name)
    {
        return new Profile(id, name);
    }

    public void UpdateName(string name)
    {
        Name = name;
        AddDomainEvent(new ProfileNameChangedDomainEvent(ClientProfileId, name));
    }
    
}