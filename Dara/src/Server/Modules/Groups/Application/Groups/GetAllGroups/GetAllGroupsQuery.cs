using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Groups.Application.Groups.GetAllGroups;

public record GetAllGroupsQuery() : IQuery<List<GroupDto>>;