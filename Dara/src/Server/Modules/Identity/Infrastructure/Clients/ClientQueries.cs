using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.Clients;

public class ClientQueries : IClientQueries
{
    private readonly IdentityContext _context;

    public ClientQueries(IdentityContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetClientByIdentifierAsync(string clientIdentifier)
    {
        return await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientIdentifier == clientIdentifier);
    }
}