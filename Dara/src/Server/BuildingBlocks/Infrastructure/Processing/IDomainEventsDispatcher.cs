using Dara.BuildingBlocks.Domain.Events;

namespace Dara.BuildingBlocks.Infrastructure.Processing;

public interface IDomainEventsDispatcher
{
    public Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
}
