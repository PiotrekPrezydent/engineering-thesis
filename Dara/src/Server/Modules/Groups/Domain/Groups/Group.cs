using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Domain.Groups.Rules;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class Group : Entity, IAggregateRoot, IHasSnapshot<GroupSnapshot>
{
    public GroupId Id { get; private set; }
    public IReadOnlyList<GroupMember> Members => _members.AsReadOnly();
    
    private List<GroupMember> _members;

    private GroupMemberId _ownerId;
    
    private string _name;

    private string _joinCode;

    private Group() { }

    private Group(GroupMemberId ownerId, string groupName, string joinCode)
    {
        Id = new GroupId(Guid.NewGuid());
        _name = groupName;
        _joinCode = joinCode;
        _ownerId = ownerId;
        _members = new List<GroupMember> { GroupMember.Create(ownerId, Id) };
        
        AddDomainEvent(new GroupCreatedDomainEvent(Id, _ownerId));
    }

    public static Group Create(GroupMemberId ownerId, string name, string joinCode)
    {
        return new Group(ownerId, name, joinCode);
    }

    public void JoinToGroup(GroupMemberId groupMemberId, string providedCode)
    {
        CheckRule(new GroupCodeMustBeValid(_joinCode, providedCode));
        
        AddMember(groupMemberId);
    }

    public void AddMember(GroupMemberId memberId)
    {
        CheckRule(new GroupMemberCannotBeAddedTwice(Members, memberId));
        
        _members.Add(GroupMember.Create(memberId, Id));
        AddDomainEvent(new NewMemberJoinedGroupDomainEvent(Id, memberId));
    }

    public void RemoveMember(GroupMemberId memberId)
    {
        CheckRule(new NotActualMemberCannotLeaveGroup(Members, memberId));
        var member = Members.Single(member => member.Id == memberId);
        
        _members.Remove(member);
        AddDomainEvent(new MemberLeftGroupDomainEvent(Id, memberId));
    }

    public GroupSnapshot GetSnapshot()
    {
        return new GroupSnapshot(Id.Value,
            _ownerId.Value,
            _name,
            _joinCode,
            _members.Select(e => e.Id.Value)
                .ToList());
    }
}