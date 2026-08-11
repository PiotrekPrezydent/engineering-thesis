using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Profiles.Integration;

public record ProfileNameChangedIntegrationEvent : IntegrationEventBase
{
    public Guid ProfileId { get; }
    public string NewName { get; }
    
    public ProfileNameChangedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid profileId, string newName) : base(eventId, occurredOn)
    {
        ProfileId = profileId;
        NewName = newName;
    }
}