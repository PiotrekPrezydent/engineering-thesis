using Dara.Server.Modules.Authorization.Domain.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Authorization.Infrastructure.Permissions;

public class PermissionRepository : IPermissionRepository
{
    private readonly AuthorizationContext _context;

    public PermissionRepository(AuthorizationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync()
    {
        return await _context.Permissions.ToListAsync();
    }

    public async Task AddAsync(Permission permission)
    {
        await _context.Permissions.AddAsync(permission);
    }

    public void Remove(Permission permission)
    {
        _context.Permissions.Remove(permission);
    }
}
