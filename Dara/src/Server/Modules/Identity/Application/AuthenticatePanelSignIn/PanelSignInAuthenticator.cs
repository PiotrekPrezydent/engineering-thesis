using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Domain.UserSessions;

namespace Dara.Server.Modules.Identity.Application.AuthenticatePanelSignIn;

// the sign-in of the panel cookie to the user id, for the authentication handler of the host; it saves the moved
// idle lifetime itself, because it runs outside the command pipeline and its unit of work
public class PanelSignInAuthenticator
{
    private readonly IUserSessionRepository _sessions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public PanelSignInAuthenticator(IUserSessionRepository sessions, IUnitOfWork unitOfWork, IdentityOptions options, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _unitOfWork = unitOfWork;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<PanelSignInResult?> AuthenticateAsync(Guid panelSignInId)
    {
        var signInId = new PanelSignInId(panelSignInId);
        var session = await _sessions.FindByPanelSignInIdAsync(signInId);
        if (session == null || !session.UsePanelSignIn(signInId, _timeProvider.GetUtcNow().UtcDateTime, _options.PanelSignInIdleLifetime))
            return null;

        await _unitOfWork.CommitAsync();
        return new PanelSignInResult(session.UserId.Value, session.ClientId.Value);
    }
}

// the account and its panel client (the subject of the cookie in Authorization)
public record PanelSignInResult(Guid UserId, Guid PanelClientId);
