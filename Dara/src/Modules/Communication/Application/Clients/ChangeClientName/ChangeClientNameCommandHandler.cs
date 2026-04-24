using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.ChangeClientName;

public class ChangeClientNameCommandHandler : IApplicationCommandHandler<ChangeClientNameCommand, ChangeClientNameCommandResult>
{
    public ChangeClientNameCommandHandler()
    {
    }
    
    public async Task<ChangeClientNameCommandResult> HandleAsync(ChangeClientNameCommand command)
    {
        return new();
    }
}