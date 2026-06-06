using Dara.Server.BuildingBlocks.Integration;

namespace Dara.BuildingBlocks.Infrastructure.IntegrationEvents;

public interface IIntegrationEventDispatcher
{
    public Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent;
}