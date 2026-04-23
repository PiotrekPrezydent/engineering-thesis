using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes.Events;
using Dara.Modules.Communication.Domain.NodesMeshes;

namespace Dara.Modules.Communication.Domain.Nodes;

public class Node : Entity, IAggregateRoot
{
    public NodeName Name { get; private set; }
    
    public NodeAuthToken AuthToken { get; private set; }
    
    public Client NodeOwner { get; private set; }

    public NodesMesh CurrentNodesMesh { get; private set; }
    

    public Node(NodeName name, NodeAuthToken authToken, Client nodeOwner)
    {
        Id = Guid.NewGuid();
        
        Name = name;
        AuthToken = authToken;
        
        NodeOwner = nodeOwner;

        CurrentNodesMesh = null;
        
        _events.Add(new EntityCreatedEvent<Node>(this, this));
    }

    public void ChangeName(NodeName newName)
    {
        Name = newName;
        
        _events.Add(new EntityValueObjectChangedEvent<Node,NodeName>(this, Name));
    }

    public void ChangeAuthToken(NodeAuthToken newAuthToken)
    {
        AuthToken = newAuthToken;
        
        _events.Add(new EntityValueObjectChangedEvent<Node,NodeAuthToken>(this, AuthToken));
    }
    
    public void JoinNodesMesh(NodesMesh nodesMesh)
    {
        if(CurrentNodesMesh != null)
            throw new InvalidOperationException("Cannot join a group more than once");
        
        CurrentNodesMesh = nodesMesh;
        
        _events.Add(new NodeJoinedNodesMeshEvent(this, CurrentNodesMesh));
    }

    public void LeaveNodesMesh(NodesMesh nodesMesh)
    {
        if(CurrentNodesMesh == null)
            throw new InvalidOperationException("Node dont have a group");
        
        CurrentNodesMesh = null;
        
        _events.Add(new NodeLeftNodesMeshEvent(this, nodesMesh));
    }
    
    
}