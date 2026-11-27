using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.UserSessions;

namespace Dara.Server.Modules.Identity.Application.RegisterClient;

// the code tells the account; the used code, the new client and its session with the first refresh token are saved
// together, so a code is used once
public class RegisterClientCommandHandler : ICommandHandler<RegisterClientCommand, ClientTokensResult>
{
    private readonly IUserRepository _users;
    private readonly IClientRepository _clients;
    private readonly IUserSessionRepository _sessions;
    private readonly IClientCodeFactory _codeFactory;
    private readonly IRefreshTokenFactory _tokenFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public RegisterClientCommandHandler(IUserRepository users, IClientRepository clients, IUserSessionRepository sessions, IClientCodeFactory codeFactory, IRefreshTokenFactory tokenFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _users = users;
        _clients = clients;
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
        if (!user.UseClientRegistrationCode(codeHash, now))
            return ClientTokensResult.Failed(AuthError.InvalidRegistrationCode);

        // a broken rule (a name too long) throws, so the unit of work saves nothing and the code stays open
        var type = ClientTypeLabels.Parse(command.ClientType);
        var sameTypeCount = await _clients.CountHubClientsAsync(user.Id, type);
        var client = Client.RegisterHub(user.Id, type, command.Name, sameTypeCount, now);
        await _clients.AddAsync(client);

        var lifetime = _options.RefreshTokenLifetime;
        var token = _tokenFactory.Create();
        await _sessions.AddAsync(UserSession.StartHub(client.Id, user.Id, token.Hash, now, lifetime));

        return ClientTokensResult.Issued(user.Id.Value, client.Id.Value, token.Token, now + lifetime);
    }
}
