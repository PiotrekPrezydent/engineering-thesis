using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class GroupMember : Entity
{
    public GroupMemberId Id { get; private set; }
    
    public GroupId GroupId { get; private set; }

    private GroupMember() { }

    private GroupMember(GroupMemberId id, GroupId groupId)
    {
        GroupId = groupId;
        Id = id;
    }

    internal static GroupMember Create(GroupMemberId memberId, GroupId groupId)
    {
        return new GroupMember(memberId, groupId);
    }

    public bool IsMemberOfGroup(GroupId groupId)
    {
        return GroupId == groupId;
    }
}