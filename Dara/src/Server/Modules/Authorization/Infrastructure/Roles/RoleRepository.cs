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

    public async Task<Role?> FindByCodeAsync(string code)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Code == code);
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
