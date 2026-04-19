using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Groups.Events;

namespace Dara.Modules.Communication.Domain.Groups;

public class Group : Entity, IAggregateRoot
{
    public Client GroupCreator { get; private set; }
    
    public GroupName Name { get; private set; }
    
    public GroupCode Code { get; private set; }
    
    public IReadOnlyList<Client> Members => _members.AsReadOnly();
    
    private readonly List<Client> _members;

    public Group(GroupName name, GroupCode code, Client creator)
    {
        Id = Guid.NewGuid();
        
        Name = name;
        Code = code;
        
        GroupCreator = creator;

        _members = new();
    }
    
    public void ChangeName(GroupName newName)
    {
        Name = newName;
        
        _events.Add(new EntityValueObjectChangedEvent<Group, GroupName>(this, Name));
    }
    
    public void ChangeCode(GroupCode newCode)
    {
        Code = newCode;
        
        _events.Add(new EntityValueObjectChangedEvent<Group, GroupCode>(this, Code));
    }
    
    public void AddMember(Client newMember)
    {
        _members.Add(newMember);
        
        _events.Add(new GroupMemberAddedEvent(this, newMember));
    }

    public void RemoveMember(Client groupMember)
    {
        _members.Remove(groupMember);
        
        if (_members.Count == 0)
        {
            _events.Add(new GroupBecameEmptyEvent(this));
            return;
        }
        
        _events.Add(new GroupMemberRemovedEvent(this, groupMember));
    }
}