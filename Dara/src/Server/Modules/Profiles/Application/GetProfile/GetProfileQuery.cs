using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Profiles.Application.GetProfile;

public record GetProfileQuery(Guid ProfileId) : IQuery<Guid>;
