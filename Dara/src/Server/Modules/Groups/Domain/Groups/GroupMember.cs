using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class GroupMember : Entity<GroupMemberId>
{
    public GroupId GroupId { get; private set; }

    private GroupMember(GroupMemberId id, GroupId groupId) : base(id)
    {
        Id = id;
        GroupId = groupId;
    }

    internal static GroupMember Create(GroupMemberId id, GroupId groupId)
    {
        return new(id, groupId);
    }
    
}