using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Profiles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Profiles.Application.GetProfile;

public class GetProfilesQueryHandler : IQueryHandler<GetProfilesQuery, List<ProfileDto>>
{
    private readonly IReadModel _readModel;

    public GetProfilesQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<ProfileDto>> HandleAsync(GetProfilesQuery query)
    {
        var profiles = await _readModel.Query<Profile>().Where(e => e.Id.MatchAny(query.ProfilesIds)).ToListAsync();
       
        
        return profiles.Select(p => new ProfileDto(p.Id.Value, p.Name)).ToList();
    }
}