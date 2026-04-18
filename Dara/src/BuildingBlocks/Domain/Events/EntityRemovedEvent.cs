using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.BuildingBlocks.Domain.Events;

public class EntityRemovedEvent<T> : BaseDomainEvent where T : Entity
{
    public T RemovedEntity { get; }

    public EntityRemovedEvent(object eventCaller, T entity) : base(eventCaller)
    {
        RemovedEntity = entity;
    }
}