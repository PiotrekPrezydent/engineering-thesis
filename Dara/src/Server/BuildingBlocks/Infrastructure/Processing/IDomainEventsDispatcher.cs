using Dara.BuildingBlocks.Domain;

namespace Dara.BuildingBlocks.Infrastructure.Processing;

public interface IDomainEventsDispatcher
{
    public Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
}
