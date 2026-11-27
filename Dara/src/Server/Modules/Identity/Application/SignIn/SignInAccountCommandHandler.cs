using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.AccountSessions;

namespace Dara.Server.Modules.Identity.Application.SignIn;

// every sign-in of the panel is a new session; it publishes nothing, the panel is not a client
public class SignInAccountCommandHandler : ICommandHandler<SignInAccountCommand, AccountSignInResult>
{
    private readonly CredentialsVerifier _credentialsVerifier;
    private readonly IAccountSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public SignInAccountCommandHandler(CredentialsVerifier credentialsVerifier, IAccountSessionRepository sessions, TimeProvider timeProvider)
    {
        _credentialsVerifier = credentialsVerifier;
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task<AccountSignInResult> HandleAsync(SignInAccountCommand command)
    {
        var user = await _credentialsVerifier.VerifyAsync(command.Login, command.Password);
        if (user == null)
            return AccountSignInResult.Failed(AuthError.InvalidCredentials);

        var session = AccountSession.Start(user.Id, _timeProvider.GetUtcNow().UtcDateTime);
        await _sessions.AddAsync(session);

        return AccountSignInResult.SignedIn(user.Id.Value, session.Id.Value);
    }
}
