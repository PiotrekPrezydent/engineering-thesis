using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Profiles.Domain;

public interface IProfileRepository : IRepository
{
    Task AddAsync(Profile profile);
    
    Task UpdateAsync(Profile profile);
    
    Task<Profile> GetByIdAsync(ProfileId profileId);
}