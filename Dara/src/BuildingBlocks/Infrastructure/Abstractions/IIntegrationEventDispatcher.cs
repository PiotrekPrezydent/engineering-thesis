using Dara.BuildingBlocks.Integration;

namespace Dara.BuildingBlocks.Infrastructure.Abstractions;

public interface IIntegrationEventDispatcher
{
    public Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent;
}