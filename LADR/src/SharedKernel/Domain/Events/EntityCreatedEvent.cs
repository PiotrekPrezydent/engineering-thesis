using LADR.SharedKernel.Domain.Models;

namespace LADR.SharedKernel.Domain.Events;

public class EntityCreatedEvent<T> : DomainEvent where T : Entity 
{
    public T Entity { get; }
    
    public EntityCreatedEvent(T entity) : base()
    {
        Entity = entity;
    } 
}