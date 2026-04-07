namespace Dara.Core.Domain.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
}