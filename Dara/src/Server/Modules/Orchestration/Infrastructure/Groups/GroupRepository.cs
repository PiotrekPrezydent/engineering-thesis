using Dara.Server.Modules.Orchestration.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Infrastructure.Groups;

public class GroupRepository : IGroupRepository
{
    private readonly OrchestrationContext _context;

    public GroupRepository(OrchestrationContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Group group)
    {
        await _context.ParticipantGroups.AddAsync(group);
    }

    public async Task<Group> GetByIdAsync(GroupId groupId)
    {
        return await _context.ParticipantGroups.FirstAsync(e=>e.Id == groupId);
    }
}