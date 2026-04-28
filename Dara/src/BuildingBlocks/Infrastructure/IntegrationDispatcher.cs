using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.BuildingBlocks.Integration;

namespace Dara.BuildingBlocks.Infrastructure;

public class IntegrationDispatcher : IIntegrationDispatcher
{
    public async Task IntegrateEventAsync(IIntegrationEvent integrationEvent)
    {
        throw new NotImplementedException();
    }
}