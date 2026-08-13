using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class NonActualMemberCannotLeaveGroupRule : IBuisnessRule
{
    private readonly IReadOnlyList<GroupMember> _members;
    private readonly MemberId _memberId;

    public NonActualMemberCannotLeaveGroupRule(IReadOnlyList<GroupMember> members, MemberId memberId)
    {
        _members = members;
        _memberId = memberId;
    }
    public string Message => nameof(NonActualMemberCannotLeaveGroupRule);
    
    public bool IsBroken()
    {
        return _members.All(e => e.MemberId != _memberId);
    }
}