using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.Application.RevokeDeviceSession;

// ends the tokens and the open login code of the device and keeps it from signing in again; a repeated
// revocation changes nothing
public class RevokeDeviceSessionCommandHandler : ICommandHandler<RevokeDeviceSessionCommand>
{
    private readonly IDeviceSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public RevokeDeviceSessionCommandHandler(IDeviceSessionRepository sessions, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(RevokeDeviceSessionCommand command)
    {
        var session = await _sessions.FindByIdAsync(new DeviceId(command.DeviceId));
        session?.Revoke(_timeProvider.GetUtcNow().UtcDateTime);
    }
}
