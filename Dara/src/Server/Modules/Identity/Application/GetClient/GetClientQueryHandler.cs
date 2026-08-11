using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Application.GetClient;

public class GetClientQueryHandler : IQueryHandler<GetClientQuery, Guid?>
{
    private readonly IReadModel _readModel;

    public GetClientQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }
    
    public async Task<Guid?> HandleAsync(GetClientQuery query)
    {
        var client = await  _readModel.Query<ClientIdentity>().FirstOrDefaultAsync(e=>e.IsIdentifiedBy(query.ClientIdentifier));
        if (client == null)
            return null;

        return client.ClientId.Value;
    }
}