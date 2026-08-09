using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain;

public interface IClientRepository : IRepository
{
    public Task<Client> GetByClientIdentifierAsync(string clientIdentifier);

    public Task AddAsync(Client client);
}