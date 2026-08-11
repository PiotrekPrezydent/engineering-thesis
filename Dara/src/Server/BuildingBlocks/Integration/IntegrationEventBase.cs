namespace Dara.Server.BuildingBlocks.Integration;

public abstract record IntegrationEventBase : IIntegrationEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    
    protected IntegrationEventBase(Guid eventId, DateTime occurredOn)
    {
        OccurredOn = occurredOn;
        EventId = eventId;
    }
}