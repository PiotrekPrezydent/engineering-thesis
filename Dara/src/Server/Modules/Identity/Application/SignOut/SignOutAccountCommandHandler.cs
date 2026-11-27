using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.UserSessions;

namespace Dara.Server.Modules.Identity.Application.SignOut;

// a sign-in of another account or an unknown one is left as it is; a repeated sign-out changes nothing
public class SignOutAccountCommandHandler : ICommandHandler<SignOutAccountCommand>
{
    private readonly IUserSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public SignOutAccountCommandHandler(IUserSessionRepository sessions, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(SignOutAccountCommand command)
    {
        var signInId = new PanelSignInId(command.PanelSignInId);
        var session = await _sessions.FindByPanelSignInIdAsync(signInId);
        if (session == null || session.UserId.Value != command.UserId)
            return;

        session.SignOutPanelSignIn(signInId, _timeProvider.GetUtcNow().UtcDateTime);
    }
}
