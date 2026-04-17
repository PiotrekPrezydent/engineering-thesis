using Dara.BuildingBlocks.Domain;
using Dara.Modules.RpcGateway.Domain.Clients;
using Dara.Modules.RpcGateway.Domain.Connections.Events;

namespace Dara.Modules.RpcGateway.Domain.Connections;

//represent connection to server
public class Connection : Entity, IAggregateRoot
{
    public ConnectionId ConnectionId { get; private set; }
    
    public ConnectionIp ConnectionIp { get; private set; }
    
    public Client? ConnectionClient { get; private set; }

    public Connection(ConnectionId connectionId, ConnectionIp connectionIp)
    {
        Id = Guid.NewGuid();
        
        ConnectionId = connectionId;
        ConnectionIp = connectionIp;

        ConnectionClient = null;
        
        _events.Add(new ConnectionCreatedEvent(this));
    }

    public void RegisterClient(Client client)
    {
        if(HasClient())
            throw new ArgumentException($"Client is already registered in this connection");
        
        ConnectionClient = client;
        _events.Add(new ConnectionClientRegisteredEvent(this));
    }
    
    public void RevokeClient()
    {
        if(!HasClient())
            throw new ArgumentException($"Client is not registered in this connection");
        
        ConnectionClient = null;
        _events.Add(new ConnectionClientRevokedEvent(this));
    }
    
    bool HasClient()
    {
        return ConnectionClient != null;
    }

}