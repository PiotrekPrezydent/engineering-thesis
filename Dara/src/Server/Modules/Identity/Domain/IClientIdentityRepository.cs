using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain;

public interface IClientIdentityRepository : IRepository
{
    public Task AddAsync(ClientIdentity clientIdentity);
    
    public Task<ClientIdentity> GetByIdAsync(ClientIdentityId clientId);
}