using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

namespace Dara.Server.Modules.Authorization.Application.Users.GetPermissions;

public class GetPermissionsQueryHandler : IQueryHandler<GetPermissionsQuery, CallerPermissionsDto>
{
    private readonly IReadModel _readModel;

    public GetPermissionsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public Task<CallerPermissionsDto> HandleAsync(GetPermissionsQuery query)
    {
        return CallerPermissions.ReadAsync(_readModel, query.UserId, query.ClientId);
    }
}
