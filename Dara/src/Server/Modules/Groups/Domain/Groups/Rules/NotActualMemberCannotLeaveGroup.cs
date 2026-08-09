using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class NotActualMemberCannotLeaveGroup : IBuisnessRule
{
    private readonly List<GroupMember> _members;
    private readonly GroupMemberId _memberId;

    public NotActualMemberCannotLeaveGroup(List<GroupMember> members, GroupMemberId memberId)
    {
        _members = members;
        _memberId = memberId;
    }
    public string Message => nameof(NotActualMemberCannotLeaveGroup);
    public bool IsBroken()
    {
        return _members.All(e => e.MemberId != _memberId);
    }
}