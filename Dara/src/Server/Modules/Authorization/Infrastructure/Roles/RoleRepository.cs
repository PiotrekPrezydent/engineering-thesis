using Dara.Server.Modules.Authorization.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Authorization.Infrastructure.Roles;

public class RoleRepository : IRoleRepository
{
    private readonly AuthorizationContext _context;

    public RoleRepository(AuthorizationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetDefaultRoleCodesAsync()
    {
        return await _context.Roles.Where(r => r.IsDefault).Select(r => r.Code).ToListAsync();
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public void Remove(Role role)
    {
        _context.Roles.Remove(role);
    }
}
