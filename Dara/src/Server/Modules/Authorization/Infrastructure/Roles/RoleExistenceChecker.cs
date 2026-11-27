using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Infrastructure.Roles;

public class RoleExistenceChecker : IRoleExistenceChecker
{
    private readonly AuthorizationContext _context;

    public RoleExistenceChecker(AuthorizationContext context)
    {
        _context = context;
    }

    public bool Exists(string roleCode)
    {
        return _context.Roles.Any(r => r.Code == roleCode);
    }
}
