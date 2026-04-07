namespace Dara.Core.Domain.Events;

public class BaseDomainEvent
{
    public Guid Id { get; }

    public DateTime Timestamp { get; }

    public BaseDomainEvent()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.Now;
    }
}