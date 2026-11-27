using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Users;

public interface IUserRolesRepository : IRepository
{
    public Task AddAsync(UserRoles userRoles);

    // null when Authorization does not know the account yet (the event of Identity is still on its way)
    public Task<UserRoles?> FindByUserIdAsync(UserId userId);

    // every account; only for a role no module declares any more, which is rare
    public Task<IReadOnlyList<UserRoles>> GetAllAsync();
}
