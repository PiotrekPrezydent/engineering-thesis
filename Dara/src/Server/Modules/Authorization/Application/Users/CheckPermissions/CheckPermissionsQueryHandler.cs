using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

namespace Dara.Server.Modules.Authorization.Application.Users.CheckPermissions;

public class CheckPermissionsQueryHandler : IQueryHandler<CheckPermissionsQuery, List<PermissionCheckDto>>
{
    private readonly IReadModel _readModel;

    public CheckPermissionsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<PermissionCheckDto>> HandleAsync(CheckPermissionsQuery query)
    {
        var caller = await CallerPermissions.ReadAsync(_readModel, query.UserId, query.ClientId);
        return query.PermissionCodes
            .Distinct()
            .Select(code => new PermissionCheckDto(code, caller.PermissionCodes.Contains(code)))
            .ToList();
    }
}
