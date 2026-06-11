using Dara.BuildingBlocks.Domain.Models;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Domain.Members;

public class Member : Entity, IAggregateRoot
{
    public MemberId Id;
    
    GroupId GroupId;

    private Member(MemberId id)
    {
        Id = id;
    }

    public static Member Create(MemberId id)
    {
        return new(id);
    }
    
    internal void JoinGroup(GroupId groupId)
    {
        GroupId = groupId;
    }
    
}