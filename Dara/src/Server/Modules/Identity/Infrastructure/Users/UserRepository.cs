using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private IdentityContext _context;

    public UserRepository(IdentityContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByUserIdentifierAsync(string userIdentifier)
    {
        User? user = _context.Users
            .FirstOrDefault(e => e.UserIdentifier == userIdentifier);
        
        return user;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}