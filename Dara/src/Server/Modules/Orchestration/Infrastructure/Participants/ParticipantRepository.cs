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
        return await _context.Participants.Include(p=>p.Functions).FirstAsync(x => x.Id == id);
    }
}