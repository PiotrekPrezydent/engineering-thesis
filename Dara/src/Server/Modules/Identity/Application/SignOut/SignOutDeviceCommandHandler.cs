using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.Application.SignOut;

// a session of another account or an unknown one is left as it is; a repeated sign-out changes nothing
public class SignOutDeviceCommandHandler : ICommandHandler<SignOutDeviceCommand>
{
    private readonly IDeviceSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public SignOutDeviceCommandHandler(IDeviceSessionRepository sessions, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(SignOutDeviceCommand command)
    {
        var session = await _sessions.FindByIdAsync(new DeviceId(command.DeviceId));
        if (session == null || session.UserId.Value != command.UserId)
            return;

        session.SignOut(_timeProvider.GetUtcNow().UtcDateTime);
    }
}
