using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Profiles.Application.GetProfile;

public record GetProfileQuery(Guid ClientId) : IQuery<ProfileDto>;

public record GetProfilesQuery(params Guid[] ClientsIds) : IQuery<List<ProfileDto>>;
