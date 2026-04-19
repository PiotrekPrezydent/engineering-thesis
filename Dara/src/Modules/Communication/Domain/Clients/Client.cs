using Dara.BuildingBlocks.Domain;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Domain.Clients;

//represent connection to server
public class Client : Entity, IAggregateRoot
{
    public ClientConnectionId ClientConnectionId { get; private set; }
    
    public ClientConnectionIp ClientConnectionIp { get; private set; }
    
    public Node? RegisteredNode { get; private set; }
    
    public Client(ClientConnectionId clientConnectionId, ClientConnectionIp clientConnectionIp)
    {
        Id = Guid.NewGuid();
        
        ClientConnectionId = clientConnectionId;
        ClientConnectionIp = clientConnectionIp;

        RegisteredNode = null;
    }

    public void RegisterNode(Node node)
    {
        if(RegisteredNode != null)
            throw new InvalidOperationException("Client is already registered");
        
        RegisteredNode = node;
    }

    public void DeregisterNode()
    {
        if(RegisteredNode == null)
            throw new InvalidOperationException("Client is not registered");
        
        RegisteredNode = null;
    }
}