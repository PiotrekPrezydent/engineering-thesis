using Dara.BuildingBlocks.Integration;

namespace Dara.BuildingBlocks.Infrastructure.Abstractions;

public interface IIntegrationDispatcher
{
    public Task IntegrateEventAsync(IIntegrationEvent integrationEvent);
}