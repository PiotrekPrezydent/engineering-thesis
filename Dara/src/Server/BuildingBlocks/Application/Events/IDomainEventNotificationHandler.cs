using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.BuildingBlocks.Application.Events;

public interface IDomainEventNotificationHandler<in TDomainEvent> where TDomainEvent : IDomainEvent 
{
    public Task HandleAsync(TDomainEvent notification);
}