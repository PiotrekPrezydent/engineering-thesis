using Dara.Server.Modules.Identity.Domain.ClientSessions;
using Dara.Server.Modules.Identity.Domain.Users.Clients;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.ClientSessions;

public class ClientSessionRepository : IClientSessionRepository
{
    private readonly IdentityContext _context;

    public ClientSessionRepository(IdentityContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ClientSession session)
    {
        await _context.ClientSessions.AddAsync(session);
    }

    public async Task<ClientSession?> FindByClientIdAsync(UserClientId clientId)
    {
        return await _context.ClientSessions.FirstOrDefaultAsync(s => s.ClientId == clientId);
    }

    public async Task<ClientSession?> FindByTokenHashAsync(string tokenHash)
    {
        return await _context.ClientSessions.FirstOrDefaultAsync(s => s.Tokens.Any(t => t.TokenHash == tokenHash));
    }
}
