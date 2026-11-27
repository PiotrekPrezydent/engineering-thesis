using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.Clients;

public class ClientRepository : IClientRepository
{
    private readonly IdentityContext _context;

    public ClientRepository(IdentityContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
    }

    public async Task<Client?> FindByIdAsync(ClientId id)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client?> FindByLoginCodeHashAsync(string codeHash)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.Hub != null && c.Hub.LoginCode != null && c.Hub.LoginCode.Hash == codeHash);
    }

    public async Task<Client> GetPanelAsync(UserId userId)
    {
        return await _context.Clients.FirstAsync(c => c.UserId == userId && c.Kind == ClientKind.Panel);
    }

    public async Task<int> CountHubClientsAsync(UserId userId, ClientType type)
    {
        return await _context.Clients.CountAsync(c => c.UserId == userId && c.Kind == ClientKind.Hub && c.Hub != null && c.Hub.Type == type);
    }
}
