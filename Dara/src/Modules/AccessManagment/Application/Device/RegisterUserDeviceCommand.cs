using Dara.Core.Domain.Commands;

namespace Dara.Modules.AccessManagment.Application.Device;

public record RegisterUserDeviceCommand(Guid userId, string deviceName) : IApplicationCommand; // device hash will be getted from name

class RegisterUserDeviceCommandHandler : IApplicationCommandHandler<RegisterUserDeviceCommand>
{
    public Task HandleAsync(RegisterUserDeviceCommand command)
    {
        throw new NotImplementedException();
    }
}