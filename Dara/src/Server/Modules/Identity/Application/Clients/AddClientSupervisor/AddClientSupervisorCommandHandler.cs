using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application.Clients.AddClientSupervisor;

public class AddClientSupervisorCommandHandler : IHandler<AddClientSupervisorCommand>
{
    public AddClientSupervisorCommandHandler()
    {
    }
    
    public async Task HandleAsync(AddClientSupervisorCommand command)
    {
        return;
    }
}