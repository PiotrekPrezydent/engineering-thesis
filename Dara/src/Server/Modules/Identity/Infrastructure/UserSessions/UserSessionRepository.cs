using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.UserSessions;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.UserSessions;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly IdentityContext _context;

    public UserSessionRepository(IdentityContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserSession session)
    {
        await _context.UserSessions.AddAsync(session);
    }

    public async Task<UserSession?> FindByClientIdAsync(ClientId clientId)
    {
        return await _context.UserSessions.FirstOrDefaultAsync(s => s.ClientId == clientId);
    }

    public async Task<UserSession?> FindByTokenHashAsync(string tokenHash)
    {
        return await _context.UserSessions.FirstOrDefaultAsync(s => s.Tokens.Any(t => t.TokenHash == tokenHash));
    }

    public async Task<UserSession?> FindByPanelSignInIdAsync(PanelSignInId signInId)
    {
        return await _context.UserSessions.FirstOrDefaultAsync(s => s.PanelSignIns.Any(p => p.Id == signInId));
    }
}
