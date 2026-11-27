using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Roles;

public interface IRoleRepository : IRepository
{
    public Task<IReadOnlyList<Role>> GetAllAsync();

    // the codes of the roles every new account and every new client gets
    public Task<IReadOnlyList<string>> GetDefaultRoleCodesAsync();

    public Task AddAsync(Role role);

    public void Remove(Role role);
}
