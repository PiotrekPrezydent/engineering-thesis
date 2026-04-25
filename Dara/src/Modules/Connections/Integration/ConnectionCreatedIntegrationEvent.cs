using Dara.BuildingBlocks.Integration;

namespace Dara.Modules.Connections.Integration;

public class ConnectionCreatedIntegrationEvent : IIntegrationEvent
{
    public string ConnectionId { get; set; }
    
    public ConnectionCreatedIntegrationEvent(string connectionId)
    {
        ConnectionId = connectionId;
    }
}