using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.UserSessions;

namespace Dara.Server.Modules.Identity.Application.SignIn;

// every sign-in of the panel is a new sign-in of a browser in the session of the panel client of the account; it
// publishes nothing, the panel client exists as long as the account does
public class SignInAccountCommandHandler : ICommandHandler<SignInAccountCommand, AccountSignInResult>
{
    private readonly CredentialsVerifier _credentialsVerifier;
    private readonly IClientRepository _clients;
    private readonly IUserSessionRepository _sessions;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public SignInAccountCommandHandler(CredentialsVerifier credentialsVerifier, IClientRepository clients, IUserSessionRepository sessions, IdentityOptions options, TimeProvider timeProvider)
    {
        _credentialsVerifier = credentialsVerifier;
        _clients = clients;
        _sessions = sessions;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<AccountSignInResult> HandleAsync(SignInAccountCommand command)
    {
        var user = await _credentialsVerifier.VerifyAsync(command.Login, command.Password);
        if (user == null)
            return AccountSignInResult.Failed(AuthError.InvalidCredentials);

        var panel = await _clients.GetPanelAsync(user.Id);
        var session = await _sessions.FindByClientIdAsync(panel.Id)
            ?? throw new InvalidOperationException($"the panel client {panel.Id.Value} has no session");

        var signIn = session.StartPanelSignIn(_timeProvider.GetUtcNow().UtcDateTime, _options.PanelSignInIdleLifetime);
        return AccountSignInResult.SignedIn(user.Id.Value, panel.Id.Value, signIn.Id.Value);
    }
}
