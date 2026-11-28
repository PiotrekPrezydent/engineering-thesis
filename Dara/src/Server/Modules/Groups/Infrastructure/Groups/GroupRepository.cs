using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public class GroupRepository : IGroupRepository
{
    private readonly GroupsContext _context;

    public GroupRepository(GroupsContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Group group)
    {
       await _context.Groups.AddAsync(group);
    }

    public async Task<Group> GetByIdAsync(GroupId groupId)
    {
        return await _context.Groups.Include(g=>g.Members).FirstOrDefaultAsync(e=>e.Id == groupId) ?? throw new EntityNotFoundException("Group", groupId);
    }
}