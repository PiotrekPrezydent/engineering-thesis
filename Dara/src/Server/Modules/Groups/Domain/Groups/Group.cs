using Dara.BuildingBlocks.Domain.Models;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class Group : Entity,  IAggregateRoot
{
    public GroupId Id;

    private string GroupName;
    private string GroupJoinCode;
    
    List<MemberId> Members;

    internal void AddMember(MemberId memberId)
    {
        
    }
    
}