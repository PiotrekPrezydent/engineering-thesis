using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.AccountSessions;

public interface IAccountSessionRepository : IRepository
{
    Task AddAsync(AccountSession session);

    Task<AccountSession?> FindByIdAsync(AccountSessionId id);
}
