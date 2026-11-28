using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Infrastructure.Participants;

public class ParticipantRepository : IParticipantRepository
{
    OrchestrationContext _context;

    public ParticipantRepository(OrchestrationContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Participant participant)
    {
        await _context.Participants.AddAsync(participant);
    }

    public async Task<Participant> GetByIdAsync(ParticipantId id)
    {
        return await _context.Participants.Include(p=>p.Functions).FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Participant", id);
    }

    public async Task<Participant?> FindByIdAsync(ParticipantId id)
    {
        return await _context.Participants.Include(p => p.Functions).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Participant>> GetByIdsAsync(IReadOnlyCollection<ParticipantId> ids)
    {
        return await _context.Participants.Include(p => p.Functions).Where(x => ids.Contains(x.Id)).ToListAsync();
    }
}