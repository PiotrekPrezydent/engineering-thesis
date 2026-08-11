using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class OnlyActualGroupMemberCanSendMessageRule : IBuisnessRule
{
    private readonly IReadOnlyList<GroupMember> _members;
    private readonly GroupMemberId _memberId;
    
    public OnlyActualGroupMemberCanSendMessageRule(IReadOnlyList<GroupMember> members, GroupMemberId memberId)
    {
        _members = members;
        _memberId = memberId;
    }

    public string Message => "Only actual group member can send message";
    public bool IsBroken()
    {
        return _members.All(e=>e.Id != _memberId);
    }
}