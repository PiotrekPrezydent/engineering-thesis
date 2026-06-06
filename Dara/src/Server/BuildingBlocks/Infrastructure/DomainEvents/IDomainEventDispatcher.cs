using System.Threading.Tasks;
using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Models;

namespace Dara.BuildingBlocks.Infrastructure.DomainEvents;

public interface IDomainEventDispatcher
{
    public Task DispatchSingleEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
    
    public Task DispatchEntityEventsAsync(Entity entity);
    
}