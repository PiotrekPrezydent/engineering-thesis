namespace Dara.Core.Shared.Models;

public class DomainEvent
{
    public Guid Id { get; }

    public DateTime Timestamp { get; }

    public DomainEvent()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.Now;
    }
}