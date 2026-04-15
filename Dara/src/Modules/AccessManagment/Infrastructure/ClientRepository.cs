using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Clients;

namespace Dara.Modules.AccessManagment.Infrastructure;

public class ClientRepository : IClientRepository
{
    private List<Client> _clients;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ClientRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _clients = new();
        _domainEventDispatcher = domainEventDispatcher;
    }
    
    public async Task<Client> FindByName(ClientName name)
    {
        return _clients.First(e => e.Name == name);
    }

    public async Task<Client> FindById(Guid id)
    {
        return _clients.First(e => e.Id == id);
    }

    public async Task Add(Client client)
    {
        _clients.Add(client);
        foreach (var domainEvent in client.DomainEvents)
        {
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        }
        client.ClearDomainEvents();
    }

    public async Task Save(Client client)
    {
        var clientToRemove = _clients.First(e => e.Id == client.Id);
        _clients.Remove(clientToRemove);
        _clients.Add(client);
        
        foreach (var domainEvent in client.DomainEvents)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvent);
        }
        client.ClearDomainEvents();
    }
}