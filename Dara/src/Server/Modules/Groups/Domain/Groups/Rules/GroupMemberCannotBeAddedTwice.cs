using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class GroupMemberCannotBeAddedTwice : IBuisnessRule
{
    private readonly IReadOnlyList<GroupMember> _members;
    private readonly GroupMemberId _memberId;

    public GroupMemberCannotBeAddedTwice(IReadOnlyList<GroupMember> members, GroupMemberId memberId)
    {
        _members = members;
        _memberId = memberId;
    }
    
    public string Message => nameof(GroupMemberCannotBeAddedTwice);
    public bool IsBroken()
    {
         return _members.Any(e=>e.Id == _memberId);
    }
}