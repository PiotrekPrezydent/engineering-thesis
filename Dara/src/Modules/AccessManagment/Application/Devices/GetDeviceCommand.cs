using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Devices;

namespace Dara.Modules.AccessManagment.Application.Devices;

public record GetDeviceCommand(string Name) : IApplicationCommand;

public record GetDeviceCommandResult(Guid DeviceId) : IApplicationCommandResult;

public class GetDeviceCommandHandler : IApplicationCommandHandler<GetDeviceCommand, GetDeviceCommandResult>
{
    private readonly IDeviceRepository _deviceRepository;

    public GetDeviceCommandHandler(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }
    
    public async Task<GetDeviceCommandResult> HandleAsync(GetDeviceCommand command)
    {
        DeviceName name = new(command.Name);
        Device device = await _deviceRepository.FindByName(name);
        
        return new GetDeviceCommandResult(device.Id);
    }
}