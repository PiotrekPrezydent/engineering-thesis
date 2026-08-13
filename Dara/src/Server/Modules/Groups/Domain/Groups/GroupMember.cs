using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class GroupMember : Entity
{
    public MemberId MemberId { get; private set; }
    public Member Member { get; private set; }
    
    public Group Group { get; private set; }
    public GroupId GroupId { get; private set; }

    private GroupMember() { }

    private GroupMember(MemberId memberId, GroupId groupId)
    {
        GroupId = groupId;
        MemberId = memberId;
    }

    internal static GroupMember Create(MemberId memberId, GroupId groupId)
    {
        return new GroupMember(memberId, groupId);
    }

    public bool IsMemberOfGroup(GroupId groupId)
    {
        return GroupId == groupId;
    }
}