using LADR.SharedKernel.Domain.Models;

namespace LADR.SharedKernel.Infrastructure.Models;

public interface IDomainEventHandler <in TEvent> where TEvent : DomainEvent
{
    Task HandleAsync(TEvent domainEvent);
}