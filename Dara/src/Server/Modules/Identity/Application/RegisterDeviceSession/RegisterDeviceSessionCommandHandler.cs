using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Sessions;

namespace Dara.Server.Modules.Identity.Application.RegisterDeviceSession;

// idempotent: a known device stays as it is, also when the command names another account (a device belongs to one
// account for good)
public class RegisterDeviceSessionCommandHandler : ICommandHandler<RegisterDeviceSessionCommand>
{
    private readonly IDeviceSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public RegisterDeviceSessionCommandHandler(IDeviceSessionRepository sessions, TimeProvider timeProvider)
    {
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(RegisterDeviceSessionCommand command)
    {
        if (await _sessions.FindByIdAsync(new DeviceId(command.DeviceId)) != null)
            return;

        var session = DeviceSession.Register(command.DeviceId, new UserId(command.UserId), _timeProvider.GetUtcNow().UtcDateTime);
        await _sessions.AddAsync(session);
    }
}
