using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.Clients;

public class ClientRepository : IClientRepository
{
    private IdentityContext _context;

    public ClientRepository(IdentityContext context)
    {
        _context = context;
    }
    
    public async Task<Client> GetByClientIdentifierAsync(string clientIdentifier)
    {
        return await _context.Clients.FirstAsync(e => e.ClientIdentifier == clientIdentifier);
    }

    public async Task AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
    }
}