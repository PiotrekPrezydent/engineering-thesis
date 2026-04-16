using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.RpcGateway.Domain;

public class ClientConnection : Entity, IAggregateRoot
{
    public ConnectionId ConnectionId { get; }
    
    public ConnectionIp ConnectionIp { get; }
    
    public ClientName ClientName { get; }
    
    public ClientAuthToken ClientAuthToken { get; }

    public ClientConnection(ConnectionId connectionId, ConnectionIp connectionIp, ClientName clientName, ClientAuthToken clientAuthToken)
    {
        Id = Guid.NewGuid();
        
        ConnectionId = connectionId;
        ConnectionIp = connectionIp;
        ClientName = clientName;
        ClientAuthToken = clientAuthToken;
    }
}