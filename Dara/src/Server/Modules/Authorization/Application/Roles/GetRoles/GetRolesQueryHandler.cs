using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Authorization.Application.Roles.GetRoles;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IReadModel _readModel;

    public GetRolesQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<RoleDto>> HandleAsync(GetRolesQuery query)
    {
        var roles = await _readModel.Query<Role>().ToListAsync();
        return roles
            .OrderBy(r => r.Module, StringComparer.Ordinal)
            .ThenBy(r => r.Code, StringComparer.Ordinal)
            .Select(r => new RoleDto(r.Code, r.Module, r.IsDefault, r.PermissionCodes.ToList()))
            .ToList();
    }
}
