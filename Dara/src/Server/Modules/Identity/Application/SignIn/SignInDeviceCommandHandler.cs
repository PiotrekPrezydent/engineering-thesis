using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.Application.SignIn;

// the code tells which device signs in; the device exists already (created in the panel), the code is used up
public class SignInDeviceCommandHandler : ICommandHandler<SignInDeviceCommand, DeviceTokensResult>
{
    private readonly IDeviceSessionRepository _sessions;
    private readonly IDeviceLoginCodeFactory _codeFactory;
    private readonly IRefreshTokenFactory _tokenFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public SignInDeviceCommandHandler(IDeviceSessionRepository sessions, IDeviceLoginCodeFactory codeFactory, IRefreshTokenFactory tokenFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _codeFactory = codeFactory;
        _tokenFactory = tokenFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<DeviceTokensResult> HandleAsync(SignInDeviceCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return DeviceTokensResult.Failed(AuthError.InvalidLoginCode);

        var codeHash = _codeFactory.Hash(command.Code);
        var session = await _sessions.FindByLoginCodeHashAsync(codeHash);
        if (session == null)
            return DeviceTokensResult.Failed(AuthError.InvalidLoginCode);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var lifetime = _options.RefreshTokenLifetime;
        var token = _tokenFactory.Create();

        return session.SignIn(codeHash, token.Hash, now, lifetime) switch
        {
            DeviceSignInOutcome.SignedIn => DeviceTokensResult.Issued(session.UserId.Value, session.Id.Value, token.Token, now + lifetime),
            DeviceSignInOutcome.DeviceRevoked => DeviceTokensResult.Failed(AuthError.DeviceRevoked),
            _ => DeviceTokensResult.Failed(AuthError.InvalidLoginCode)
        };
    }
}
