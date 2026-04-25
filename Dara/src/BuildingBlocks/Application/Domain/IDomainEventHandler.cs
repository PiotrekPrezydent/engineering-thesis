using Dara.BuildingBlocks.Domain.Events.Abstraction;

namespace Dara.BuildingBlocks.Application.Domain
{
    public interface IDomainEventHandler<in TEvent> : IDomainEventHandler where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent);
    }

    public interface IDomainEventHandler;
}