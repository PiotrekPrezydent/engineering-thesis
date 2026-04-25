using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.BuildingBlocks.Application.Abstraction
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent);
    }
}