using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Domain.Groups.Rules;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class Group : Entity, IAggregateRoot
{
    public GroupId Id { get; private set; }
    public IReadOnlyList<GroupMember> Members => _members.AsReadOnly();
    
    private List<GroupMember> _members;

    public GroupMember Creator { get; private set; }
    public MemberId CreatorId { get; private set; }

    public string Name { get; private set; }

    public string JoinCode { get; private set; }

    private Group() { }

    private Group(MemberId creatorId, string groupName, string joinCode)
    {
        Id = new GroupId(Guid.NewGuid());
        Name = groupName;
        JoinCode = joinCode;
        CreatorId = creatorId;
        _members = new List<GroupMember> { GroupMember.Create(creatorId, Id) };
        
        AddDomainEvent(new GroupCreatedDomainEvent(Id, CreatorId));
        AddDomainEvent(new NewMemberJoinedGroupDomainEvent(Id, CreatorId));
    }

    public static Group Create(MemberId creatorId, string name, string joinCode)
    {
        return new Group(creatorId, name, joinCode);
    }

    public void JoinMemberToGroup(MemberId memberId, string providedCode)
    {
        CheckRule(new ProvidedGroupJoiningCodeMustBeValidRule(providedCode, JoinCode));
        CheckRule(new GroupMemberCannotBeAddedTwiceRule(Members, memberId));
        
        _members.Add(GroupMember.Create(memberId, Id));
        
        AddDomainEvent(new NewMemberJoinedGroupDomainEvent(Id, memberId));
    }

    public void LeaveGroup(MemberId memberId)
    {
        CheckRule(new NonActualMemberCannotLeaveGroupRule(Members, memberId));
        var member = Members.Single(member => member.MemberId == memberId);
        
        _members.Remove(member);
        AddDomainEvent(new MemberLeftGroupDomainEvent(Id, memberId));
    }

    public GroupMessage SendMessageToGroup(MemberId authorId, string message)
    {
        CheckRule(new OnlyActualGroupMemberCanSendMessageRule(_members,authorId));
        
        return GroupMessage.Create(Id, authorId, message);
    }
    
}