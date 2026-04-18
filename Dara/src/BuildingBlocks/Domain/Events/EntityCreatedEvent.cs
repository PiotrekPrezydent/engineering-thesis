using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.BuildingBlocks.Domain.Events;

public class EntityCreatedEvent<T> : BaseDomainEvent where T : Entity
{
    public T CreatedEntity { get; }

    public EntityCreatedEvent(object eventCaller, T entity) : base(eventCaller)
    {
        CreatedEntity = entity;
    }
}