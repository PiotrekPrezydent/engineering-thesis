using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Models;
using Dara.Shared.Common;

namespace Dara.BuildingBlocks.Infrastructure.Abstractions;

public interface IDomainEventDispatcher
{
    public Task DispatchSingleEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
    
    public Task DispatchEntityEventsAsync(Entity entity);
    
}