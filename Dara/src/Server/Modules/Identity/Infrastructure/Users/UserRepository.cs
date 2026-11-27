using Dara.Server.Modules.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private readonly IdentityContext _context;

    public UserRepository(IdentityContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(UserId id)
    {
        return await _context.Users.FirstAsync(e => e.Id == id);
    }

    public async Task<User?> FindByClientRegistrationCodeHashAsync(string codeHash)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.ClientRegistrationCodes.Any(c => c.Hash == codeHash));
    }

    public async Task<User?> FindByClientLoginCodeHashAsync(string codeHash)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Clients.Any(c => c.LoginCode != null && c.LoginCode.Hash == codeHash));
    }

    public async Task<User?> FindByLoginAsync(string login)
    {
        var normalized = User.NormalizeLogin(login);
        return await _context.Users.FirstOrDefaultAsync(e => e.Login == normalized);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}
