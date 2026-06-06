using Dara.BuildingBlocks.Domain.Models.Abstraction;

namespace Dara.Server.Modules.Identity.Domain.Clients;

public interface IClientRepository : IRepository
{
    public Task AddAsync(Client client);
    public Task UpdateAsync(Client client);
    public Task DeleteAsync(Client client);
    public Task<Client> GetAsync(Client client);
}