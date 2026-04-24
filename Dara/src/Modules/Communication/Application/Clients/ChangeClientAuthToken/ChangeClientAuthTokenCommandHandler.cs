using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.ChangeClientAuthToken;

public class ChangeClientAuthTokenCommandHandler : IApplicationCommandHandler<ChangeClientAuthTokenCommand, ChangeClientAuthTokenCommandResult>
{
    public ChangeClientAuthTokenCommandHandler()
    {
    }
    
    public async Task<ChangeClientAuthTokenCommandResult> HandleAsync(ChangeClientAuthTokenCommand command)
    {
        return new();
    }
}