using Dara.BuildingBlocks.Infrastructure.Processing;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Infrastructure;

public class ClientRepository : IClientRepository
{
    List<Client> _clients;
    readonly IUnitOfWork<> _unitOfWork;
    
    public ClientRepository(IDomainEventDispatcher domainEventDispatcher, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _clients = new List<Client>();
    }
    
    public async Task Add(Client client)
    {
        _clients.Add(client);
        
        _unitOfWork.Attach(client);   
    }

    public async Task SaveAsync(Client client)
    {
        var existingClient = _clients.FirstOrDefault(c => c.Id == client.Id);
        
        if (existingClient == null)
            throw new DomainEntityNotFoundException(_clients, client.Id);
        
        var index = _clients.IndexOf(existingClient);
        _clients[index] = client;
        
        _unitOfWork.Attach(client);   
    }

    public async Task DeleteAsync(Client client)
    {
        var existingClient = _clients.FirstOrDefault(c => c.Id == client.Id);
        
        if (existingClient == null)
            throw new DomainEntityNotFoundException(_clients, client.Id);
        
        _clients.Remove(existingClient);
        
        _unitOfWork.Attach(client);   
    }

    public async Task<Client> GetByClientIdAsync(ClientId clientId)
    {
        var existingClient = _clients.FirstOrDefault(c => c.Id == clientId);
        
        if (existingClient == null)
            throw new DomainEntityNotFoundException(_clients, clientId);
        
        _unitOfWork.Attach(existingClient);  
        
        return existingClient;
    }
    
}