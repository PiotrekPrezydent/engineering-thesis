using Dara.Server.Modules.Profiles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Profiles.Infrastructure.Profiles;

public class ProfileRepository : IProfileRepository
{
    private ProfilesContext _context;

    public ProfileRepository(ProfilesContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Profile profile)
    {
        await _context.Profiles.AddAsync(profile);
    }
    
    public async Task<Profile> GetByIdAsync(ProfileId profileId)
    {
        return await _context.Profiles.FirstAsync(e => e.Id == profileId);
    }
}