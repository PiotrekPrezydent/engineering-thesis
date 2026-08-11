using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.Clients;

public class ClientIdentityRepository : IClientIdentityRepository
{
    private IdentityContext _contextBase;

    public ClientIdentityRepository(IdentityContext contextBase)
    {
        _contextBase = contextBase;
    }
    
    public async Task<ClientIdentity> GetByIdAsync(ClientIdentityId clientId)
    {
        return await _contextBase.Clients.FirstAsync(e => e.ClientId == clientId);
    }

    public async Task AddAsync(ClientIdentity clientIdentity)
    {
        await _contextBase.Clients.AddAsync(clientIdentity);
    }
}