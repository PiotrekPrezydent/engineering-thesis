using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class GroupMemberCannotBeAddedTwiceRule : IBuisnessRule
{
    private readonly IReadOnlyList<GroupMember> _members;
    private readonly MemberId _memberId;

    public GroupMemberCannotBeAddedTwiceRule(IReadOnlyList<GroupMember> members, MemberId memberId)
    {
        _members = members;
        _memberId = memberId;
    }
    
    public string Message => nameof(GroupMemberCannotBeAddedTwiceRule);
    public bool IsBroken()
    {
         return _members.Any(e=>e.MemberId == _memberId);
    }
}