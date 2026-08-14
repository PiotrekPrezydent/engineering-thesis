using Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Infrastructure.ParticipantGroups;

public class ParticipantGroupRepository : IParticipantGroupRepository
{
    private readonly OrchestrationContext _context;

    public ParticipantGroupRepository(OrchestrationContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ParticipantGroup group)
    {
        await _context.ParticipantGroups.AddAsync(group);
    }

    public async Task<ParticipantGroup> GetByIdAsync(ParticipantGroupId groupId)
    {
        return await _context.ParticipantGroups.FirstAsync(e=>e.Id == groupId);
    }
}