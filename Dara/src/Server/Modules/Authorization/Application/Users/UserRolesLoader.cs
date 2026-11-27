using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;

namespace Dara.Server.Modules.Authorization.Application.Users;

internal static class UserRolesLoader
{
    // an account Authorization does not know yet (its event of Identity is still on its way) starts with the default roles
    public static async Task<UserRoles> GetOrCreateAsync(this IUserRolesRepository repository, IRoleRepository roleRepository, UserId userId)
    {
        var userRoles = await repository.FindByUserIdAsync(userId);
        if (userRoles is not null)
            return userRoles;

        userRoles = UserRoles.Create(userId, await roleRepository.GetDefaultRoleCodesAsync());
        await repository.AddAsync(userRoles);
        return userRoles;
    }
}
