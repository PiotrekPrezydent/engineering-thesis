using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Domain.ClientSessions;

public interface IClientSessionRepository : IRepository
{
    Task AddAsync(ClientSession session);

    Task<ClientSession?> FindByClientIdAsync(UserClientId clientId);

    // any token of the session, used or not, so a reuse is recognized
    Task<ClientSession?> FindByTokenHashAsync(string tokenHash);
}