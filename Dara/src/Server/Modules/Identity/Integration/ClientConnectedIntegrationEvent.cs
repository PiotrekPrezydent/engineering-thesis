using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

// the first hub connection of a client opened; the host publishes it straight on the bus, without an outbox: presence is volatile and after
// a restart every client is offline anyway. Sequence grows over the host, a receiver drops an event older than the
// last one it saw for the client
public record ClientConnectedIntegrationEvent : IntegrationEventBase
{
    public Guid ClientId { get; }
    public Guid UserId { get; }
    public long Sequence { get; }

    public ClientConnectedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid clientId, Guid userId, long sequence) : base(eventId, occurredOn)
    {
        ClientId = clientId;
        UserId = userId;
        Sequence = sequence;
    }
}
