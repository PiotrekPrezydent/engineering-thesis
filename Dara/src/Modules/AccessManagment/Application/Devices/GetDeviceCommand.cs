using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Devices;

namespace Dara.Modules.AccessManagment.Application.Devices;

public record GetDeviceCommand(string Name) : IApplicationCommand;

public record GetDeviceCommandresult(Guid DeviceId) : IApplicationCommandResult;

public class GetDeviceCommandHandler : IApplicationCommandHandler<GetDeviceCommand, GetDeviceCommandresult>
{
    private readonly IDeviceRepository _deviceRepository;

    public GetDeviceCommandHandler(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }
    
    public async Task<GetDeviceCommandresult> HandleAsync(GetDeviceCommand command)
    {
        DeviceName name = new(command.Name);
        Device device = await _deviceRepository.FindByName(name);
        
        return new GetDeviceCommandresult(device.Id);
    }
}