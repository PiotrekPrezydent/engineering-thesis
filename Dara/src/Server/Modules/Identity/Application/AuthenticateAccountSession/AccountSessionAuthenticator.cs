using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Domain.AccountSessions;

namespace Dara.Server.Modules.Identity.Application.AuthenticateAccountSession;

// the session of the panel cookie to the user id, for the authentication handler of the host; it saves the moved
// idle lifetime itself, because it runs outside the command pipeline and its unit of work
public class AccountSessionAuthenticator
{
    private readonly IAccountSessionRepository _sessions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public AccountSessionAuthenticator(IAccountSessionRepository sessions, IUnitOfWork unitOfWork, IdentityOptions options, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _unitOfWork = unitOfWork;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<Guid?> AuthenticateAsync(Guid accountSessionId)
    {
        var session = await _sessions.FindByIdAsync(new AccountSessionId(accountSessionId));
        if (session == null || !session.Use(_timeProvider.GetUtcNow().UtcDateTime, _options.AccountSessionIdleLifetime))
            return null;

        await _unitOfWork.CommitAsync();
        return session.UserId.Value;
    }
}
