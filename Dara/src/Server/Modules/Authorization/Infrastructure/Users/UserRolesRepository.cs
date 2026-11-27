using Dara.Server.Modules.Authorization.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Authorization.Infrastructure.Users;

public class UserRolesRepository : IUserRolesRepository
{
    private readonly AuthorizationContext _context;

    public UserRolesRepository(AuthorizationContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserRoles userRoles)
    {
        await _context.UserRoles.AddAsync(userRoles);
    }

    public async Task<UserRoles?> FindByUserIdAsync(UserId userId)
    {
        return await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<IReadOnlyList<UserRoles>> GetAllAsync()
    {
        return await _context.UserRoles.ToListAsync();
    }
}
