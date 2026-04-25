using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.BuildingBlocks.Infrastructure.Domain
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    }
}