using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Application.GetGroupDetails;

public class GetGroupDetailsQueryHandler : IQueryHandler<GetGroupDetailsQuery,GroupDetailsDto>
{
    private readonly IReadModel _readModel;

    public GetGroupDetailsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<GroupDetailsDto> HandleAsync(GetGroupDetailsQuery query)
    {
        var group = await _readModel
            .Query<Group>()
            .Include(g=>g.Members)
            .FirstAsync(e => e.GroupId.Value == query.GroupId);
        
        var g = new GroupDetailsDto(group.GroupId.Value, group.OwnerId.Value, group.Name, group.JoinCode,
            group.Members.Select(e=>e.MemberId.Value).ToList());
        return g;
    }
}