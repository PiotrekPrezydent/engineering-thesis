using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Application.GetUser;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, Guid?>
{
    private readonly IReadModel _readModel;

    public GetUserQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }
    
    public async Task<Guid?> HandleAsync(GetUserQuery query)
    {
        var client =  _readModel.Query<User>().ToList().FirstOrDefault(e=>e.IsIdentifiedBy(query.UserIdentifier));
        if (client == null)
            return null;

        return client.Id.Value;
    }
}