using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private IdentityContext _context;

    public UserRepository(IdentityContext context)
    {
        _context = context;
    }
    
    public async Task<User> GetByIdAsync(UserId id)
    {
        return await _context.Users.FirstAsync(e => e.Id == id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}