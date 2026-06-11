using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Infrastructure;

public class ClientRepository : IClientRepository
{
    readonly ClientsContext _clientsContext;
    public ClientRepository(ClientsContext clientsContext)
    {
        _clientsContext = clientsContext;
    }
    public async Task Add(Client client)
    {
        _clientsContext.Clients.Add(client);
    }

    public async Task<Client> GetByClientIdAsync(ClientId clientId)
    {
        throw new NotImplementedException();
    }
}
