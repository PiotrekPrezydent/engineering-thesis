using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

public record UserClientRenamedIntegrationEvent : IntegrationEventBase
{
    public Guid ClientId { get; }
    public Guid UserId { get; }
    public string NewName { get; }

    public UserClientRenamedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid clientId, Guid userId, string newName) : base(eventId, occurredOn)
    {
        ClientId = clientId;
        UserId = userId;
        NewName = newName;
    }
}
