using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Application.GetAllGroups;

public class GetAllGroupsQueryHandler : IQueryHandler<GetAllGroupsQuery,List<GroupDto>>
{
    private readonly IReadModel _readModel;

    public GetAllGroupsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<GroupDto>> HandleAsync(GetAllGroupsQuery query)
    {
        return _readModel.Query<Group>().Select(e => new GroupDto(e.GroupId, e.Name)).ToList();
    }
}