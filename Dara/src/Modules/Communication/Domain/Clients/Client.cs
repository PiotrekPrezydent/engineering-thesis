using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Domain.Clients;

//represent connection to server
public class Client : Entity, IAggregateRoot
{
    public ClientConnectionId ClientConnectionId { get; private set; }
    
    public ClientConnectionIp ClientConnectionIp { get; private set; }
    
    public Node? ClientNode { get; private set; }
    
    public Client(ClientConnectionId clientConnectionId, ClientConnectionIp clientConnectionIp)
    {
        Id = Guid.NewGuid();
        
        ClientConnectionId = clientConnectionId;
        ClientConnectionIp = clientConnectionIp;

        ClientNode = null;
        
        _events.Add(new EntityCreatedEvent<Client>(this,this));
    }

    public void EnableNode(Node node)
    {
        if(ClientNode != null)
            throw new InvalidOperationException("Client is already enabled");
        
        ClientNode = node;
    }

    public void DisableNode()
    {
        if(ClientNode == null)
            throw new InvalidOperationException("Client is not enabled");
        
        ClientNode = null;
    }
}