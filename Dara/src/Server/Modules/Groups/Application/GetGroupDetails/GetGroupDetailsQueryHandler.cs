using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
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
            .Include(g => g.Members)
            .FirstAsync(e => e.Id.Value == query.GroupId);
        
        var snapshot = group.GetSnapshot();
        var g = new GroupDetailsDto(snapshot.GroupId, snapshot.OwnerId, snapshot.Name, snapshot.JoinCode,
            snapshot.Members);
        return g;
    }
}