using Dara.Server.Modules.Clients.Application.Clients.AddClientSupervisor;
using Dara.Server.Modules.Clients.Application.Clients.ChangeClientName;
using Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOffline;
using Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOnline;
using Dara.Server.Modules.Clients.Application.Clients.RemoveClientSupervisor;

namespace Dara.Server.Apps.API.Hubs;

public partial class AppHub
{
    public async Task GoOnline()
    {
        var command = new MarkClientAsOnlineCommand(CallerId);
        await _clientsModule.ExecuteCommandAsync(command);
    }

    public async Task GoOffline()
    {
        var command = new MarkClientAsOfflineCommand(CallerId);
        await _clientsModule.ExecuteCommandAsync(command);
    }

    public async Task ChangeName(string newName)
    {
        var command = new ChangeClientNameCommand(CallerId, newName);
        await _clientsModule.ExecuteCommandAsync(command);
    }

    public async Task AddSupervisor(Guid supervisorId)
    {
        var command = new AddClientSupervisorCommand(CallerId, supervisorId);
        await _clientsModule.ExecuteCommandAsync(command);
    }

    public async Task RemoveSupervisor(Guid supervisorId)
    {
        var command = new RemoveClientSupervisorCommand(CallerId, supervisorId);
        await _clientsModule.ExecuteCommandAsync(command);
    }
}