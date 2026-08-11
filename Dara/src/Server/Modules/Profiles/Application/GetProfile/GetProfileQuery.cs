using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Profiles.Application.GetProfile;

public record GetProfileQuery(Guid ProfileId) : IQuery<ProfileDto>;

public record GetProfilesQuery(IEnumerable<Guid> ProfilesIds) : IQuery<List<ProfileDto>>;
