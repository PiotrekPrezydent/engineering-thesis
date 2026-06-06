using Dara.BuildingBlocks.Infrastructure.DomainEvents;
using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class ClientRepository : IClientRepository
{
    List<Client> _clients;
    readonly IDomainEventDispatcher _domainEventDispatcher;
    
    public ClientRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _clients = new List<Client>();
    }
    
    public async Task AddAsync(Client client)
    {
        await _domainEventDispatcher.DispatchEntityEventsAsync(client);
        
        _clients.Add(client);
    }

    public async Task UpdateAsync(Client client)
    {
        var existingClient = _clients.FirstOrDefault(c => c.Id == client.Id);
        
        if (existingClient == null)
            throw new KeyNotFoundException($"Client not found: {client.Id}");
        
        var index = _clients.IndexOf(existingClient);
        
        await _domainEventDispatcher.DispatchEntityEventsAsync(client);
        
        _clients[index] = client;
    }

    public async Task DeleteAsync(Client client)
    {
        var existingClient = _clients.FirstOrDefault(c => c.Id == client.Id);
        
        if (existingClient == null)
            throw new KeyNotFoundException($"Client not found: {client.Id}");
        
        await _domainEventDispatcher.DispatchEntityEventsAsync(client);
        
        _clients.Remove(existingClient);
    }

    public async Task<Client> GetAsync(Client client)
    {
        var existingClient = _clients.FirstOrDefault(c => c.Id == client.Id);
        
        if (existingClient == null)
            throw new KeyNotFoundException($"Client not found: {client.Id}");
        
        return existingClient;
    }
    
}