using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages;

public class OnlyActualGroupMemberCanSendMessage : IBuisnessRule
{
    private readonly Group _group;
    private readonly GroupMember _sender;

    public OnlyActualGroupMemberCanSendMessage(Group group, GroupMember sender)
    {
        _group = group;
        _sender = sender;
    }
    public string Message => "Only actual group member can send message";
    public bool IsBroken()
    {
        return !_sender.IsMember(_group.GroupId);
    }
}