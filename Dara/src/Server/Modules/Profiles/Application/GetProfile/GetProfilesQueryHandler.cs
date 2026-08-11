using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Profiles.Domain;

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
        var profiles = _readModel.Query<Profile>()
            .Where(e => query.ClientsIds.Contains(e.ProfileId.Value))
            .Select(p => p.GetSnapshot())
            .Select(p => new ProfileDto(p.ClientId, p.ClientName));
        
        return profiles.ToList();
    }
}