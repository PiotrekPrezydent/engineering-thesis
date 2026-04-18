namespace Dara.BuildingBlocks.Domain.Events.Abstractions;

public class BaseDomainEvent : IDomainEvent
{
    public object EventCreator { get; protected set; }
    
    public BaseDomainEvent(object eventCreator)
    {
        EventCreator = eventCreator;
    }
}