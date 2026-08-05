using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;

public interface IDomainEventsDispatcher
{
    public Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
}
