using Dara.BuildingBlocks.Domain.Exceptions;

namespace Dara.BuildingBlocks.Domain;

public abstract class Entity
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    readonly List<IDomainEvent> _domainEvents = new();

    //EF constructor
    protected Entity()
    {
        
    }
        
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
    public TId ClientId { get; protected set; }
    
    protected Entity(TId clientId)
    {
        ClientId = clientId;
    }
}