using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Application.GetValidGroup;

public class GetValidGroupQueryHandler : IQueryHandler<GetValidGroupQuery,Guid?>
{
    private readonly IReadModel _readModel;

    public GetValidGroupQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<Guid?> HandleAsync(GetValidGroupQuery query)
    {
        var group = await _readModel
            .Query<Group>()
            .Include(g => g.Members)
            .FirstOrDefaultAsync(e => e.JoinCode == query.JoinCode);
        
        if (group == null)
            return null;
        return group.GroupId;
    }
}