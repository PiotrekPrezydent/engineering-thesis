using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain;

public class GroupService : IGroupsService
{
    public void AddMemberToGroup(Member member, Group group)
    {
        //check rules for member, group.
        member.JoinGroup(group.Id);
        group.AddMember(member.Id);
    }
}