using Dara.BuildingBlocks.Domain;
using Dara.Modules.RpcGateway.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain;

public class ClientConnection : Entity, IAggregateRoot
{
    public ConnectionId ConnectionId { get; private set; }
    
    public ConnectionIp ConnectionIp { get; private set; }
    
    public ClientName ClientName { get; private set; }
    
    public ClientAuthToken ClientAuthToken { get; private set; }

    public ClientConnection(ConnectionId connectionId, ConnectionIp connectionIp, ClientName clientName, ClientAuthToken clientAuthToken)
    {
        Id = Guid.NewGuid();
        
        ConnectionId = connectionId;
        ConnectionIp = connectionIp;
        ClientName = clientName;
        ClientAuthToken = clientAuthToken;
        
        _events.Add(new ClientConnectionCreatedEvent(this));
    }

    public void ChangeClientAuthToken(ClientAuthToken clientAuthToken)
    {
        ClientAuthToken = clientAuthToken;
    }
    
    public void ChangeClientName(ClientName clientName)
    {
        ClientName = clientName;
    }
}