using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.UserSessions;
using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Application.SignOut;

// a session of another account or an unknown one is left as it is; a repeated sign-out changes nothing
public class SignOutClientCommandHandler : ICommandHandler<SignOutClientCommand>
{
    private readonly IUserSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public SignOutClientCommandHandler(IUserSessionRepository sessions, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(SignOutClientCommand command)
    {
        var session = await _sessions.FindByClientIdAsync(new ClientId(command.ClientId));
        if (session == null || session.UserId.Value != command.UserId)
            return;

        session.SignOut(_timeProvider.GetUtcNow().UtcDateTime);
    }
}
