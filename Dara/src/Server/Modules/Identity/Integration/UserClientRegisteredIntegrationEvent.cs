using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

// a client registered itself with a code of the panel, with everything the projections need at once; ClientType is
// the name of the type (Pc, Phone, Tablet, Watch, Other)
public record UserClientRegisteredIntegrationEvent : IntegrationEventBase
{
    public Guid ClientId { get; }
    public Guid UserId { get; }
    public string Name { get; }
    public string ClientType { get; }

    public UserClientRegisteredIntegrationEvent(Guid eventId, DateTime occurredOn, Guid clientId, Guid userId, string name, string clientType) : base(eventId, occurredOn)
    {
        ClientId = clientId;
        UserId = userId;
        Name = name;
        ClientType = clientType;
    }
}
