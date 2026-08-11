using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages;

public class OnlyActualGroupMemberCanSendMessage : IBuisnessRule
{
    private readonly Group _group;
    private readonly GroupMember _providedMessageAuthor;

    public OnlyActualGroupMemberCanSendMessage(Group group, GroupMember providedMessageAuthor)
    {
        _group = group;
        _providedMessageAuthor = providedMessageAuthor;
    }
    public string Message => "Only actual group member can send message";
    public bool IsBroken()
    {
        return !_providedMessageAuthor.IsMemberOfGroup(_group.Id);
    }
}