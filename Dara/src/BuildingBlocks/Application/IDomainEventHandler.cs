using Dara.BuildingBlocks.Domain.Events;

namespace Dara.BuildingBlocks.Application;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent);
}