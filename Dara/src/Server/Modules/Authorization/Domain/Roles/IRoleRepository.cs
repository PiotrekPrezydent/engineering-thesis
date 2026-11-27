using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Roles;

public interface IRoleRepository : IRepository
{
    public Task<IReadOnlyList<Role>> GetAllAsync();

    public Task<Role?> FindByCodeAsync(string code);

    public Task AddAsync(Role role);

    public void Remove(Role role);
}
