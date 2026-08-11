using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public class GroupRepository : IGroupRepository
{
    private readonly GroupContext _contextBase;

    public GroupRepository(GroupContext contextBase)
    {
        _contextBase = contextBase;
    }

    public async Task AddAsync(Group group)
    {
       await _contextBase.Groups.AddAsync(group);
    }

    public async Task<Group> GetByIdAsync(GroupId groupId)
    {
        return await _contextBase.Groups.Include(g=>g.Members).FirstAsync(e=>e.GroupId == groupId);
    }
}