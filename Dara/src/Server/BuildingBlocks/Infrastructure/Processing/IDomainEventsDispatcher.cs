using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing;

public interface IDomainEventsDispatcher
{
    public Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
}
