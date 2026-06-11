using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Exceptions;
using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.BuildingBlocks.Domain.Models;

public abstract class Entity
{
    public BaseEntityId Id { get; }
        
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    readonly List<IDomainEvent> _domainEvents = new();

    protected Entity(BaseEntityId id)
    {
        Id = id;
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