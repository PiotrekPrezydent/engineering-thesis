using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Groups.Application.Groups.GetAvaibleGroups;

public record GetAvaibleGroupsQuery() : IQuery<List<AvaibleGroupDto>>;