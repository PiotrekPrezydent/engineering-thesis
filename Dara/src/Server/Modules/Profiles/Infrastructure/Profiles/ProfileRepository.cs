using Dara.Server.Modules.Profiles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Profiles.Infrastructure.Profiles;

public class ProfileRepository : IProfileRepository
{
    private ProfilesContext _contextBase;

    public ProfileRepository(ProfilesContext contextBase)
    {
        _contextBase = contextBase;
    }

    public async Task AddAsync(Profile profile)
    {
        await _contextBase.Profiles.AddAsync(profile);
    }
    
    public async Task<Profile> GetByIdAsync(ProfileId profileId)
    {
        return await _contextBase.Profiles.FirstAsync(e => e.ProfileId == profileId);
    }
}