using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

public class UserUniquenessChecker : IUserUniquenessChecker
{
    private readonly IdentityContext _context;

    public UserUniquenessChecker(IdentityContext context)
    {
        _context = context;
    }

    public bool IsLoginTaken(string normalizedLogin)
    {
        return _context.Users.Any(u => u.Login == normalizedLogin);
    }

    public bool IsEmailTaken(string normalizedEmail)
    {
        return _context.Users.Any(u => u.NormalizedEmail == normalizedEmail);
    }
}
