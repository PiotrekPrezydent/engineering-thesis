using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Profiles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Profiles.Application.GetProfile;

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, ProfileDto>
{
    private readonly IReadModel _readModel;

    public GetProfileQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }


    public async Task<ProfileDto> HandleAsync(GetProfileQuery query)
    {
        var profile = await _readModel.Query<Profile>().FirstAsync(e=>e.ProfileId.Value ==  query.ClientId);
        
        return new ProfileDto(profile.ProfileId.Value, profile.Name);
    }
}