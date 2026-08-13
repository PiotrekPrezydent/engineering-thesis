using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;

public record GetGroupDetailsQuery(Guid GroupId) : IQuery<GroupDetailsDto>;