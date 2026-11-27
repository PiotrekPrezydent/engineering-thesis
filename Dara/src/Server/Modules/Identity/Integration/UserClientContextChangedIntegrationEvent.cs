using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

// the full dictionary; orchestration adds the context of the executor to the prompt, so it holds no secrets
public record UserClientContextChangedIntegrationEvent : IntegrationEventBase
{
    public Guid ClientId { get; }
    public Guid UserId { get; }
    public IReadOnlyDictionary<string, string> Context { get; }

    public UserClientContextChangedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid clientId, Guid userId, IReadOnlyDictionary<string, string> context) : base(eventId, occurredOn)
    {
        ClientId = clientId;
        UserId = userId;
        Context = context;
    }
}
