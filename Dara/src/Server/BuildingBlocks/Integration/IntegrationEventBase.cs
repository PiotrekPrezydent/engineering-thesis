namespace Dara.Server.BuildingBlocks.Integration;

public abstract record IntegrationEventBase : IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    
    protected IntegrationEventBase(Guid id, DateTime occurredOn)
    {
        OccurredOn = occurredOn;
        Id = id;
    }
}