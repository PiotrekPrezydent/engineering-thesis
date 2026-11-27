using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Authorization.Application.Users;

internal static class CallerPermissions
{
    // with a client the roles of the client count, without one (the panel) the roles of the account; an account or a
    // client Authorization does not know yet gets the default roles, because Identity has signed the caller in already
    // and its event is still on its way (a revoked client is refused by Identity before it gets here)
    public static async Task<CallerPermissionsDto> ReadAsync(IReadModel readModel, Guid userId, Guid? clientId)
    {
        var id = new UserId(userId);
        var userRoles = await readModel.Query<UserRoles>().FirstOrDefaultAsync(u => u.UserId == id);
        var roles = await readModel.Query<Role>().ToListAsync();

        var roleCodes = clientId is null
            ? userRoles?.AccountRoleCodes
            : userRoles?.Clients.FirstOrDefault(c => c.ClientId == new UserClientId(clientId.Value))?.RoleCodes;
        roleCodes ??= roles.Where(r => r.IsDefault).Select(r => r.Code).ToList();

        var callerRoles = roles.Where(r => roleCodes.Contains(r.Code)).ToList();
        return new CallerPermissionsDto(
            callerRoles.Select(r => r.Code).Order(StringComparer.Ordinal).ToList(),
            callerRoles.SelectMany(r => r.PermissionCodes).Distinct().Order(StringComparer.Ordinal).ToList());
    }
}
