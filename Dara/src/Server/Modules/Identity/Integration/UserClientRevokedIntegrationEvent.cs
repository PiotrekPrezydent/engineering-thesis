using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

// final: the session of the client has ended already; the host refuses its token and closes its hub connections
public record UserClientRevokedIntegrationEvent : IntegrationEventBase
{
    public Guid ClientId { get; }
    public Guid UserId { get; }

    public UserClientRevokedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid clientId, Guid userId) : base(eventId, occurredOn)
    {
        ClientId = clientId;
        UserId = userId;
    }
}
