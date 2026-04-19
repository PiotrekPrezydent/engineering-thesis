using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.Communication.Domain.Nodes;
using Dara.Modules.Communication.Domain.NodesMeshs.Events;

namespace Dara.Modules.Communication.Domain.NodesMeshs;

public class NodesMesh : Entity, IAggregateRoot
{
    public Node GroupCreator { get; private set; }
    
    public NodesMeshName Name { get; private set; }
    
    public NodesMeshCode Code { get; private set; }
    
    public IReadOnlyList<Node> Members => _members.AsReadOnly();
    
    private readonly List<Node> _members;

    public NodesMesh(NodesMeshName name, NodesMeshCode code, Node creator)
    {
        Id = Guid.NewGuid();
        
        Name = name;
        Code = code;
        
        GroupCreator = creator;

        _members = new();
    }
    
    public void ChangeName(NodesMeshName newName)
    {
        Name = newName;
        
        _events.Add(new EntityValueObjectChangedEvent<NodesMesh, NodesMeshName>(this, Name));
    }
    
    public void ChangeCode(NodesMeshCode newCode)
    {
        Code = newCode;
        
        _events.Add(new EntityValueObjectChangedEvent<NodesMesh, NodesMeshCode>(this, Code));
    }
    
    public void AddMember(Node newMember)
    {
        _members.Add(newMember);
        
        _events.Add(new NodesMeshMemberAddedEvent(this, newMember));
    }

    public void RemoveMember(Node groupMember)
    {
        _members.Remove(groupMember);
        
        if (_members.Count == 0)
        {
            _events.Add(new NodesMeshBecameEmptyEvent(this));
            return;
        }
        
        _events.Add(new NodesMeshMemberRemovedEvent(this, groupMember));
    }
}