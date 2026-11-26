using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.Application.IssueDeviceLoginCode;

// a device of another account looks like an unknown one; a new code replaces the previous one of the device
public class IssueDeviceLoginCodeCommandHandler : ICommandHandler<IssueDeviceLoginCodeCommand, DeviceLoginCodeResult>
{
    private readonly IDeviceSessionRepository _sessions;
    private readonly IDeviceLoginCodeFactory _codeFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public IssueDeviceLoginCodeCommandHandler(IDeviceSessionRepository sessions, IDeviceLoginCodeFactory codeFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _codeFactory = codeFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<DeviceLoginCodeResult> HandleAsync(IssueDeviceLoginCodeCommand command)
    {
        var session = await _sessions.FindByIdAsync(new DeviceId(command.DeviceId));
        if (session == null || session.UserId.Value != command.UserId)
            return DeviceLoginCodeResult.Failed(AuthError.DeviceNotFound);

        if (session.IsRevoked)
            return DeviceLoginCodeResult.Failed(AuthError.DeviceRevoked);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var lifetime = _options.DeviceLoginCodeLifetime;
        var code = _codeFactory.Create();

        session.IssueLoginCode(code.Hash, now, lifetime);
        return DeviceLoginCodeResult.Issued(code.Code, now + lifetime);
    }
}
