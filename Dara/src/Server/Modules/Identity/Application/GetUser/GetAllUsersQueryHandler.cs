using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Application.GetUser;

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery,List<(Guid userId, string userIdentifier)>>
{
    private readonly IReadModel _readModel;

    public GetAllUsersQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<(Guid userId, string userIdentifier)>> HandleAsync(GetAllUsersQuery query)
    {
        var users = _readModel.Query<User>().ToList().Select(e => (
            e.Id.Value,
            e.Identifier
        )).ToList();
        
        return users;
    }
}