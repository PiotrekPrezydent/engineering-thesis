namespace Dara.Server.BuildingBlocks.Integration.Authorization;

// a resource is gone for good (a revoked client, a removed plugin): every assignment on it disappears, and every
// assignment it held as a subject, so nobody keeps access to something that does not exist
public record ScopeRemovedIntegrationEvent : IntegrationEventBase
{
    public string ScopeCode { get; }
    public Guid ScopeId { get; }

    public ScopeRemovedIntegrationEvent(Guid eventId, DateTime occurredOn, string scopeCode, Guid scopeId) : base(eventId, occurredOn)
    {
        ScopeCode = scopeCode;
        ScopeId = scopeId;
    }
}
