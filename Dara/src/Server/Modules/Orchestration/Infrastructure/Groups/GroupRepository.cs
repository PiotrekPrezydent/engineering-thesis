using Dara.Server.BuildingBlocks.Domain.Exceptions;
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
        return await _context.ParticipantGroups.FirstOrDefaultAsync(e=>e.Id == groupId) ?? throw new EntityNotFoundException("Group", groupId);
    }

    public async Task<Group?> FindByIdAsync(GroupId groupId)
    {
        return await _context.ParticipantGroups.FirstOrDefaultAsync(e => e.Id == groupId);
    }
}