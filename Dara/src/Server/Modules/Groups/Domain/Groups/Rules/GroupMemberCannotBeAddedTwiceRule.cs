using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class GroupMemberCannotBeAddedTwiceRule : IBuisnessRule
{
    private readonly IReadOnlyList<GroupMember> _members;
    private readonly GroupMemberId _memberId;

    public GroupMemberCannotBeAddedTwiceRule(IReadOnlyList<GroupMember> members, GroupMemberId memberId)
    {
        _members = members;
        _memberId = memberId;
    }
    
    public string Message => nameof(GroupMemberCannotBeAddedTwiceRule);
    public bool IsBroken()
    {
         return _members.Any(e=>e.Id == _memberId);
    }
}