using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Groups.Application.GetAllGroups;

public record GetAllGroupsQuery() : IQuery<List<GroupDto>>;