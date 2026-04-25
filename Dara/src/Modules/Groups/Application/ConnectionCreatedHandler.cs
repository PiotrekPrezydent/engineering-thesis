using Dara.BuildingBlocks.Application.Integration;
using Dara.Modules.Connections.Integration;

namespace Dara.Modules.Groups.Application;

public class ConnectionCreatedHandler : IIntegrationEventHandler<ConnectionCreatedIntegrationEvent>
{
    public ConnectionCreatedHandler()
    {
        
    }
    
    public async Task HandleAsync(ConnectionCreatedIntegrationEvent integrationEvent)
    {
        Console.Write($"INT HANDLER :::: {nameof(ConnectionCreatedHandler)}.{nameof(HandleAsync)} :: {integrationEvent.ConnectionId}");
    }
}