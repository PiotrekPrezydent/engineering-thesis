using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.UserSessions;

namespace Dara.Server.Modules.Identity.Application.SignIn;

// the code tells which client signs in; the code is used up in Client and the session of the client gets new tokens
// in the same unit of work (D20); the tokens of the earlier copy of the app end
public class SignInClientCommandHandler : ICommandHandler<SignInClientCommand, ClientTokensResult>
{
    private readonly IClientRepository _clients;
    private readonly IUserSessionRepository _sessions;
    private readonly IClientCodeFactory _codeFactory;
    private readonly IRefreshTokenFactory _tokenFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public SignInClientCommandHandler(IClientRepository clients, IUserSessionRepository sessions, IClientCodeFactory codeFactory, IRefreshTokenFactory tokenFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _clients = clients;
        _sessions = sessions;
        _codeFactory = codeFactory;
        _tokenFactory = tokenFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<ClientTokensResult> HandleAsync(SignInClientCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return ClientTokensResult.Failed(AuthError.InvalidLoginCode);

        var codeHash = _codeFactory.Hash(command.Code);
        var client = await _clients.FindByLoginCodeHashAsync(codeHash);
        if (client == null)
            return ClientTokensResult.Failed(AuthError.InvalidLoginCode);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        if (!client.ConsumeLoginCode(codeHash, now))
            return ClientTokensResult.Failed(AuthError.InvalidLoginCode);

        var session = await _sessions.FindByClientIdAsync(client.Id);
        if (session == null)
            return ClientTokensResult.Failed(AuthError.InvalidLoginCode);

        var lifetime = _options.RefreshTokenLifetime;
        var token = _tokenFactory.Create();
        session.SignIn(token.Hash, now, lifetime);

        return ClientTokensResult.Issued(client.UserId.Value, client.Id.Value, token.Token, now + lifetime);
    }
}
