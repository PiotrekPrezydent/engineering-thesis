using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Domain.Clients;

public interface IClientRepository : IRepository
{
    Task AddAsync(Client client);

    Task<Client?> FindByIdAsync(ClientId id);

    // the client of the hub whose open login code has this hash
    Task<Client?> FindByLoginCodeHashAsync(string codeHash);

    // every account has exactly one, created with it
    Task<Client> GetPanelAsync(UserId userId);

    // clients of the hub of the account with this type, revoked ones too (the numbering of default names)
    Task<int> CountHubClientsAsync(UserId userId, ClientType type);
}
