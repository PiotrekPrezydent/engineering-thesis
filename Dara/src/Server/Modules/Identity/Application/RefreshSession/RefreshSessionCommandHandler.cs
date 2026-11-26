using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.Application.RefreshSession;

// rotation: the presented token is used up and a new one is issued; a refusal is saved too (a reuse ends the session)
public class RefreshSessionCommandHandler : ICommandHandler<RefreshSessionCommand, DeviceTokensResult>
{
    private readonly IDeviceSessionRepository _sessions;
    private readonly IRefreshTokenFactory _tokenFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public RefreshSessionCommandHandler(IDeviceSessionRepository sessions, IRefreshTokenFactory tokenFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _tokenFactory = tokenFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<DeviceTokensResult> HandleAsync(RefreshSessionCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return DeviceTokensResult.Failed(AuthError.RefreshInvalid);

        var presentedHash = _tokenFactory.Hash(command.RefreshToken);
        var session = await _sessions.FindByTokenHashAsync(presentedHash);
        if (session == null)
            return DeviceTokensResult.Failed(AuthError.RefreshInvalid);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var lifetime = _options.RefreshTokenLifetime;
        var newToken = _tokenFactory.Create();

        return session.Refresh(presentedHash, newToken.Hash, now, lifetime) switch
        {
            RefreshOutcome.Refreshed => DeviceTokensResult.Issued(session.UserId.Value, session.Id.Value, newToken.Token, now + lifetime),
            RefreshOutcome.Revoked => DeviceTokensResult.Failed(AuthError.RefreshRevoked),
            RefreshOutcome.Reused => DeviceTokensResult.Failed(AuthError.RefreshReused),
            RefreshOutcome.Expired => DeviceTokensResult.Failed(AuthError.RefreshExpired),
            _ => DeviceTokensResult.Failed(AuthError.RefreshInvalid)
        };
    }
}
