using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;

public interface IParticipantGroupRepository : IRepository
{
    public Task AddAsync(ParticipantGroup group);
    
    public Task<ParticipantGroup> GetByIdAsync(ParticipantGroupId groupId);
}