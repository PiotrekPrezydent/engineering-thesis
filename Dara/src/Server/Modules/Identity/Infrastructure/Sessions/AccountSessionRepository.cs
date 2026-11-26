using Dara.Server.Modules.Identity.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.Sessions;

public class AccountSessionRepository : IAccountSessionRepository
{
    private readonly IdentityContext _context;

    public AccountSessionRepository(IdentityContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AccountSession session)
    {
        await _context.AccountSessions.AddAsync(session);
    }

    public async Task<AccountSession?> FindByIdAsync(AccountSessionId id)
    {
        return await _context.AccountSessions.FirstOrDefaultAsync(s => s.Id == id);
    }
}
