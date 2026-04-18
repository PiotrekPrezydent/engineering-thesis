using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.Clients;

public class ClientConnection : ValueObject
{
    public string ConnectionId { get; }
    
    public ClientConnection(string connectionId)
    {
        ConnectionId = connectionId;
    }
}