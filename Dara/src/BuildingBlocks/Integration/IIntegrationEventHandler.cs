namespace Dara.BuildingBlocks.Integration;

public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent integrationEvent);
}

public interface IIntegrationEventHandler; // generic arg hack