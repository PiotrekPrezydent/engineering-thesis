using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants;

public interface IParticipantRepository : IRepository
{
    public Task AddAsync(Participant participant);
    
    public Task<Participant> GetByIdAsync(ParticipantId id);
}