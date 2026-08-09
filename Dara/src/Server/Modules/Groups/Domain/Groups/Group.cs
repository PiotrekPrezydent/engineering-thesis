using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Domain.Groups.Rules;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class Group : Entity, IAggregateRoot
{
    public GroupId GroupId { get; private set; }
    
    public string Name { get; private set; }
    
    public string JoinCode  { get; private set; }
    
    public List<GroupMember> Members { get;private set; }
    
    public GroupMemberId OwnerId { get; private set; }

    private Group()
    {
    }

    private Group(GroupMemberId ownerId, string groupName, string joinCode)
    {
        GroupId = new GroupId(Guid.NewGuid());
        Name = groupName;
        JoinCode = joinCode;
        OwnerId = ownerId;
        Members = new List<GroupMember> { GroupMember.Create(GroupId, ownerId) };
        
        AddDomainEvent(new GroupCreatedDomainEvent(GroupId, OwnerId));
    }

    public static Group Create(GroupMemberId ownerId, string name, string joinCode)
    {
        return new Group(ownerId, name, joinCode);
    }

    public void JoinToGroup(GroupMemberId groupMemberId, string providedCode)
    {
        CheckRule(new GroupCodeMustBeValid(JoinCode, providedCode));
        
        AddMember(groupMemberId);
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