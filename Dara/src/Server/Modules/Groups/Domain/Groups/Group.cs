using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Domain.Groups.Rules;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class Group : Entity, IAggregateRoot
{
    public GroupId GroupId { get; }
    
    public string Name { get; }
    
    public string JoinCode  { get; }
    
    public List<GroupMember> Members { get; }
    
    public GroupMember Owner { get; set; }

    private Group(GroupMemberId ownerId, string groupName, string joinCode)
    {
        GroupId = new GroupId(Guid.NewGuid());
        Name = groupName;
        JoinCode = joinCode;
        Owner = GroupMember.Create(GroupId, ownerId);
        Members = new List<GroupMember> { Owner };
        
        AddDomainEvent(new GroupCreatedDomainEvent(GroupId, Owner.MemberId));
    }

    public static Group Create(GroupMemberId ownerId, string name, string joinCode)
    {
        return new Group(ownerId, name, joinCode);
    }

    public bool IsCodeValid(string code)
    {
        return code == JoinCode;
    }

    public void AddMember(GroupMemberId memberId)
    {
        CheckRule(new GroupMemberCannotBeAddedTwice(Members, memberId));
        
        Members.Add(GroupMember.Create(GroupId, memberId));
        AddDomainEvent(new GroupMemberJoinedDomainEvent(GroupId, memberId));
    }

    public void RemoveMember(GroupMemberId memberId)
    {
        CheckRule(new NotActualMemberCannotLeaveGroup(Members, memberId));
        var member = Members.Single(member => member.MemberId == memberId);
        Members.Remove(member);
        AddDomainEvent(new GroupMemberLeftDomainEvent(GroupId, memberId));
    }
}