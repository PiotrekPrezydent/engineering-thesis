using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Application.Groups.GetAvaibleGroups;

public class GetAvaibleGroupsQueryHandler : IQueryHandler<GetAvaibleGroupsQuery,List<AvaibleGroupDto>>
{
    private readonly IReadModel _readModel;

    public GetAvaibleGroupsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<AvaibleGroupDto>> HandleAsync(GetAvaibleGroupsQuery query)
    {
        return await _readModel
            .Query<Group>()
            .Select(e=>new AvaibleGroupDto(e.Id.Value, e.Name))
            .ToListAsync();
    }
}