using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.Modules.Profiles.Domain;

namespace Dara.Server.Modules.Profiles.Application;

public interface IProfileQueries : IQueryHelper
{
    public Task<Profile> GetProfileByIdAsync(ProfileId id);
}