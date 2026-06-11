using Dara.Server.Modules.Clients.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Clients.Infrastructure.Clients;

public class ClientRepository : IClientRepository
{
    readonly ClientsContext _clientsContext;
    public ClientRepository(ClientsContext clientsContext)
    {
        _clientsContext = clientsContext;
    }
    public async Task AddAsync(Client client)
    {
        await _clientsContext.Clients.AddAsync(client);
    }

    public async Task DeleteAsync(Client client)
    {
        _clientsContext.Clients.Remove(client);
        await _clientsContext.SaveChangesAsync();
    }

    public async Task<Client> GetByClientByIdAsync(ClientId clientId)
    {
        return await _clientsContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
    }
}
