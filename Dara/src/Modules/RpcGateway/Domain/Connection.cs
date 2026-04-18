using Dara.BuildingBlocks.Domain;
using Dara.Modules.RpcGateway.Domain.Events;

namespace Dara.Modules.RpcGateway.Domain;

//represent connection to server
public class Connection : Entity, IAggregateRoot
{
    public ConnectionId ConnectionId { get; private set; }
    
    public ConnectionIp ConnectionIp { get; private set; }
    
    public Connection(ConnectionId connectionId, ConnectionIp connectionIp)
    {
        Id = Guid.NewGuid();
        
        ConnectionId = connectionId;
        ConnectionIp = connectionIp;
    }

    //here we can add validation if needed
    public void BecomeEstablished()
    {
        _events.Add(new ConnectionEstablishedEvent(this));
    }

    public void BecomeLost()
    {
        _events.Add(new ConnectionLostEvent(this));
    }
}