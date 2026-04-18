namespace Dara.BuildingBlocks.Domain.Events.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
}