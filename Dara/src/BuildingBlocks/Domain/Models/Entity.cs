using Dara.BuildingBlocks.Domain.Events.Abstraction;
using Dara.BuildingBlocks.Domain.Exceptions;
using Dara.BuildingBlocks.Domain.Rules.Abstraction;

namespace Dara.BuildingBlocks.Domain.Models;

public abstract class Entity
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    readonly List<IDomainEvent> _domainEvents = new();
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    protected void CheckRule(IDomainRule rule)
    {
        if (rule.IsBroken())
            throw new DomainRuleValidationException(rule);
    }
}