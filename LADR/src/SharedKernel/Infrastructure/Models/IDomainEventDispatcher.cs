using LADR.SharedKernel.Domain.Models;

namespace LADR.SharedKernel.Infrastructure.Models;

public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : DomainEvent;
}