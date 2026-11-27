using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.ClientSessions;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Application.RegisterClient;

// the code tells the account; the new client and its session with the first refresh token are saved together
public class RegisterClientCommandHandler : ICommandHandler<RegisterClientCommand, ClientTokensResult>
{
    private readonly IUserRepository _users;
    private readonly IClientSessionRepository _sessions;
    private readonly IClientCodeFactory _codeFactory;
    private readonly IRefreshTokenFactory _tokenFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public RegisterClientCommandHandler(IUserRepository users, IClientSessionRepository sessions, IClientCodeFactory codeFactory, IRefreshTokenFactory tokenFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _users = users;
        _sessions = sessions;
        _codeFactory = codeFactory;
        _tokenFactory = tokenFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<ClientTokensResult> HandleAsync(RegisterClientCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return ClientTokensResult.Failed(AuthError.InvalidRegistrationCode);

        var codeHash = _codeFactory.Hash(command.Code);
        var user = await _users.FindByClientRegistrationCodeHashAsync(codeHash);
        if (user == null)
            return ClientTokensResult.Failed(AuthError.InvalidRegistrationCode);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var client = user.RegisterClient(codeHash, ClientTypeLabels.Parse(command.ClientType), command.Name, now);
        if (client == null)
            return ClientTokensResult.Failed(AuthError.InvalidRegistrationCode);

        var lifetime = _options.RefreshTokenLifetime;
        var token = _tokenFactory.Create();
        await _sessions.AddAsync(ClientSession.Start(client.Id, user.Id, token.Hash, now, lifetime));

        return ClientTokensResult.Issued(user.Id.Value, client.Id.Value, token.Token, now + lifetime);
    }
}
