using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.BuildingBlocks.Domain.Events;

public class EntityValueObjectChangedEvent<TEntity, TValueObject> : IDomainEvent where TEntity : Entity where TValueObject : ValueObject
{
    public TEntity ChangedEntity { get; }
    
    public TValueObject ChangedValueObject { get; }
    
    public EntityValueObjectChangedEvent(TEntity changedEntity, TValueObject changedValueObject)
    {
        ChangedEntity = changedEntity;
        ChangedValueObject = changedValueObject;
    }
}