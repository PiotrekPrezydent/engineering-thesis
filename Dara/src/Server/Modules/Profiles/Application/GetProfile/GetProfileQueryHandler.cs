using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.Modules.Profiles.Domain;

namespace Dara.Server.Modules.Profiles.Application.GetProfile;

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, ProfileDto>
{
    private readonly IProfileQueries _profileQueries;

    public GetProfileQueryHandler(IProfileQueries profileQueries)
    {
        _profileQueries = profileQueries;
    }


    public async Task<ProfileDto> HandleAsync(GetProfileQuery query)
    {
        var profile = await _profileQueries.GetProfileByIdAsync(new ProfileId(query.ClientId));
        
        return new ProfileDto(profile.ClientProfileId.Value, profile.Name);
    }
}