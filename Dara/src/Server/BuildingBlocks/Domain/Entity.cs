using Dara.Server.BuildingBlocks.Domain.Exceptions;

namespace Dara.Server.BuildingBlocks.Domain;

public abstract class Entity
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    readonly List<IDomainEvent> _domainEvents = new();
    
    protected Entity(){}
        
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    protected void CheckRule(IBuisnessRule rule)
    {
        if (rule.IsBroken())
            throw new BuisnessRuleValidationException(rule);
    }
}

public abstract class Entity<TId> : Entity where TId : IEntityId
{
    public TId Id { get; protected set; }
    
    protected Entity(TId id)
    {
        Id = id;
    }
}