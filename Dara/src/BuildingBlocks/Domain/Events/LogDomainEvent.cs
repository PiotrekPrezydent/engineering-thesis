using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.BuildingBlocks.Domain.Events;

public class LogDomainEvent : BaseDomainEvent
{
    public Guid Id { get; }
    
    public object EventPayload { get; }

    public DateTime Timestamp { get; }

    public LogDomainEvent(object eventCaller, object eventPayload) : base(eventCaller)
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.Now;
        
        EventPayload = eventPayload;
    }
}