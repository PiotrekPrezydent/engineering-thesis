using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class GroupMember : Entity
{
    public GroupMemberId MemberId { get; }
    
    public GroupId GroupId { get; }
    
    private GroupMember(GroupId groupId, GroupMemberId memberId)
    {
        GroupId = groupId;
        MemberId = memberId;
    }

    internal static GroupMember Create(GroupId groupId, GroupMemberId memberId)
    {
        return new GroupMember(groupId, memberId);
    }

    public bool IsMember(GroupId groupId)
    {
        return GroupId == groupId;
    }
}