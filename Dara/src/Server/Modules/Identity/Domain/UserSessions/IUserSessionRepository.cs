using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Domain.UserSessions;

public interface IUserSessionRepository : IRepository
{
    Task AddAsync(UserSession session);

    // one session per client, of the hub or the panel
    Task<UserSession?> FindByClientIdAsync(ClientId clientId);

    // any token of a session of the hub, used or not, so a reuse is recognized
    Task<UserSession?> FindByTokenHashAsync(string tokenHash);

    // the session of the panel with this sign-in of a browser
    Task<UserSession?> FindByPanelSignInIdAsync(PanelSignInId signInId);
}
