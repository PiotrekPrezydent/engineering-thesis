using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain;

public interface IGroupsService
{
    public void AddMemberToGroup(Member member, Group group);
}