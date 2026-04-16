using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Devices;

namespace Dara.Modules.AccessManagment.Application.Devices;

public record AddDeviceCommand(string Name, string Token) : IApplicationCommand;

public record AddDeviceCommandResult(Guid DeviceId) : IApplicationCommandResult;

public class AddDeviceCommandHandler : IApplicationCommandHandler<AddDeviceCommand,AddDeviceCommandResult>
{
    private readonly IDeviceRepository _deviceRepository;
    
    public AddDeviceCommandHandler(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }
    
    public async Task<AddDeviceCommandResult> HandleAsync(AddDeviceCommand command)
    {
        DeviceName name = new(command.Name);
        DeviceToken token = new(command.Token);

        Device device = new(name, token);
        
        await _deviceRepository.Add(device);
        
        return new AddDeviceCommandResult(device.Id);
    }
}