using Dara.Core.Domain.Events;

namespace Dara.Core.Domain.Business;

public interface IAggregateRoot
{
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents  { get; }
    
    public void ClearDomainEvents();
}