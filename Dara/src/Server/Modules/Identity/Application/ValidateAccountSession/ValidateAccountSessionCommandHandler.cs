using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.Application.ValidateAccountSession;

public class ValidateAccountSessionCommandHandler : ICommandHandler<ValidateAccountSessionCommand, Guid?>
{
    private readonly IAccountSessionRepository _sessions;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public ValidateAccountSessionCommandHandler(IAccountSessionRepository sessions, IdentityOptions options, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<Guid?> HandleAsync(ValidateAccountSessionCommand command)
    {
        var session = await _sessions.FindByIdAsync(new AccountSessionId(command.AccountSessionId));
        if (session == null || !session.Use(_timeProvider.GetUtcNow().UtcDateTime, _options.AccountSessionIdleLifetime))
            return null;

        return session.UserId.Value;
    }
}
