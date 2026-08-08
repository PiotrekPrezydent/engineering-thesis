namespace Dara.Server.BuildingBlocks.Domain.Events;

public abstract record DomainEventBase() : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccuredOn { get; } = DateTime.UtcNow;
}