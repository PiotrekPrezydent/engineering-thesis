using Dara.BuildingBlocks.Domain;

namespace Dara.Server.BuildingBlocks.Application.Events;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    public Task HandleAsync(TEvent domainEvent);
}