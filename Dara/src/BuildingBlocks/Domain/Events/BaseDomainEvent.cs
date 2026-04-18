namespace Dara.BuildingBlocks.Domain.Events;

public class BaseDomainEvent : IDomainEvent
{
    public Guid Id { get; }

    public DateTime Timestamp { get; }

    public BaseDomainEvent()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.Now;
    }
}