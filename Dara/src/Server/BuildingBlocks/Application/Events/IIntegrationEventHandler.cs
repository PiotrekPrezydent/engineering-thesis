using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.BuildingBlocks.Application.Events;

public interface IIntegrationEventHandler;
public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler where TEvent : IIntegrationEvent 
{
    public Task HandleAsync(TEvent integrationEvent);
}