using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.ClientSessions;

namespace Dara.Server.Modules.Identity.Application.RefreshSession;

// rotation: the presented token is used up and a new one is issued; a refusal is saved too (a reuse ends the session)
public class RefreshSessionCommandHandler : ICommandHandler<RefreshSessionCommand, ClientTokensResult>
{
    private readonly IClientSessionRepository _sessions;
    private readonly IRefreshTokenFactory _tokenFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public RefreshSessionCommandHandler(IClientSessionRepository sessions, IRefreshTokenFactory tokenFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _tokenFactory = tokenFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<ClientTokensResult> HandleAsync(RefreshSessionCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return ClientTokensResult.Failed(AuthError.RefreshInvalid);

        var presentedHash = _tokenFactory.Hash(command.RefreshToken);
        var session = await _sessions.FindByTokenHashAsync(presentedHash);
        if (session == null)
            return ClientTokensResult.Failed(AuthError.RefreshInvalid);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var lifetime = _options.RefreshTokenLifetime;
        var newToken = _tokenFactory.Create();

        return session.Refresh(presentedHash, newToken.Hash, now, lifetime) switch
        {
            RefreshOutcome.Refreshed => ClientTokensResult.Issued(session.UserId.Value, session.ClientId.Value, newToken.Token, now + lifetime),
            RefreshOutcome.Revoked => ClientTokensResult.Failed(AuthError.RefreshRevoked),
            RefreshOutcome.Reused => ClientTokensResult.Failed(AuthError.RefreshReused),
            RefreshOutcome.Expired => ClientTokensResult.Failed(AuthError.RefreshExpired),
            _ => ClientTokensResult.Failed(AuthError.RefreshInvalid)
        };
    }
}
