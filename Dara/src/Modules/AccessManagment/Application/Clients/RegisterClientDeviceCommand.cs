using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Clients;
using Dara.Modules.AccessManagment.Domain.Devices;

namespace Dara.Modules.AccessManagment.Application.Clients;

public record RegisterClientDeviceCommand(Guid clientId, Guid deviceId) : IApplicationCommand;

public record RegisterClientDeviceCommandResult() : IApplicationCommandResult;

public class RegisterClientDeviceCommandHandler : IApplicationCommandHandler<RegisterClientDeviceCommand, RegisterClientDeviceCommandResult>
{
    private readonly IClientRepository _clientRepository;
    private readonly IDeviceRepository _deviceRepository;

    public RegisterClientDeviceCommandHandler(IClientRepository clientRepository, IDeviceRepository deviceRepository)
    {
        _clientRepository = clientRepository;
        _deviceRepository = deviceRepository;
    }
    
    public async Task<RegisterClientDeviceCommandResult> HandleAsync(RegisterClientDeviceCommand command)
    {
        var client = await _clientRepository.FindById(command.clientId);
        var device = await _deviceRepository.FindById(command.deviceId);
        
        device.AddClient(client);
        await _deviceRepository.Save(device);
        
        return new RegisterClientDeviceCommandResult();
    }
}