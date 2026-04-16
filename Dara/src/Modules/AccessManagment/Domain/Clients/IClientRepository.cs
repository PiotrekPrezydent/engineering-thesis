using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Clients;

public interface IClientRepository : IRepository<Client>
{
    public Task<Client> FindByName(ClientName name);
    
    public Task<Client> FindById(Guid id);

    public Task Add(Client client);
    
    public Task Save(Client client);
}