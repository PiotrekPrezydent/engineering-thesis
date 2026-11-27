using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Permissions;

public interface IPermissionRepository : IRepository
{
    public Task<IReadOnlyList<Permission>> GetAllAsync();

    public Task AddAsync(Permission permission);

    public void Remove(Permission permission);
}
