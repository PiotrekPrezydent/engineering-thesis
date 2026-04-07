using Dara.Core.Domain.Commands;

namespace Dara.Modules.AccessManagment.Application.Device;

public record RevokeUserDeviceCommand(Guid userId, string deviceName) : IApplicationCommand;

class RevokeUserDeviceCommandHandler : IApplicationCommandHandler<RevokeUserDeviceCommand>
{
    public Task HandleAsync(RevokeUserDeviceCommand command)
    {
        throw new NotImplementedException();
    }
}