using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.BuildingBlocks.Application.Events;

public interface IIntegrationEventHandler<in TEvent> where TEvent : IIntegrationEvent
{
    public Task HandleAsync(TEvent domainEvent);
}