using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Profiles.Infrastructure.Profiles;

public class ProfileQueries : IProfileQueries
{
    private readonly ProfilesContext _context;

    public ProfileQueries(ProfilesContext context)
    {
        _context = context;
    }

    public async Task<Profile> GetProfileByIdAsync(ProfileId id)
    {
        return await _context.Profiles.AsNoTracking().FirstAsync(e => e.ClientProfileId == id);
    }
}