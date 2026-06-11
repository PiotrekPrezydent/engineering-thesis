using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups.Events;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class Group : Entity<GroupId>,  IAggregateRoot
{
    public string Name  { get;  }
    public string JoinCode { get; }

    private readonly List<GroupMember> _groupMembers;
    public GroupMemberId OwnerId  { get; }

    private Group(GroupMemberId ownerId, string name, string joinCode) : base(new GroupId(Guid.NewGuid()))
    {
        Name = name;
        JoinCode = joinCode;
        OwnerId = ownerId;
        _groupMembers = new();
        
        var owner = GroupMember.Create(ownerId,Id);
        _groupMembers.Add(owner);
        
        AddDomainEvent(new GroupCreatedDomainEvent());
    }

    public static Group Create(GroupMemberId ownerId, string groupName, string groupJoinCode)
    {
        return new Group(ownerId, groupName, groupJoinCode);
        
    }
    
    public void Join(GroupMember groupMember)
    {
        _groupMembers.Add(groupMember);
        
        AddDomainEvent(new GroupMemberJoinedDomainEvent());
    }

    public void Leave(GroupMember groupMember)
    {
        _groupMembers.Remove(groupMember);
        
        AddDomainEvent(new GroupMemberLeftDomainEvent());
    }
    
    public void ChangeName(string newName)
    {
    }

    public void ChangeCode(string newCode)
    {
        
    }

    public void SendMessage(GroupMemberId groupMemberId, string message)
    {
        GroupMessage.Create(Id, groupMemberId, message);
    }
}