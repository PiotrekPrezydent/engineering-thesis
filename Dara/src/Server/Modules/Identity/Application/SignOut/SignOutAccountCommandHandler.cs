using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.AccountSessions;

namespace Dara.Server.Modules.Identity.Application.SignOut;

// a session of another account or an unknown one is left as it is; a repeated sign-out changes nothing
public class SignOutAccountCommandHandler : ICommandHandler<SignOutAccountCommand>
{
    private readonly IAccountSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public SignOutAccountCommandHandler(IAccountSessionRepository sessions, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(SignOutAccountCommand command)
    {
        var session = await _sessions.FindByIdAsync(new AccountSessionId(command.AccountSessionId));
        if (session == null || session.UserId.Value != command.UserId)
            return;

        session.SignOut(_timeProvider.GetUtcNow().UtcDateTime);
    }
}
