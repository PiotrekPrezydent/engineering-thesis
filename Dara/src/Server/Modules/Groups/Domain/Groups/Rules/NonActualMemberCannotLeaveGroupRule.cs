using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class NonActualMemberCannotLeaveGroupRule : IBuisnessRule
{
    private readonly IReadOnlyList<GroupMember> _members;
    private readonly GroupMemberId _memberId;

    public NonActualMemberCannotLeaveGroupRule(IReadOnlyList<GroupMember> members, GroupMemberId memberId)
    {
        _members = members;
        _memberId = memberId;
    }
    public string Message => nameof(NonActualMemberCannotLeaveGroupRule);
    
    public bool IsBroken()
    {
        return _members.All(e => e.Id != _memberId);
    }
}