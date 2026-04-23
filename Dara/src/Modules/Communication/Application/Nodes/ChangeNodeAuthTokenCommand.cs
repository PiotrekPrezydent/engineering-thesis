using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes;

public record ChangeNodeAuthTokenCommand() : IApplicationCommand;

public record ChangeNodeAuthTokenCommandResult() : IApplicationCommandResult;

public class ChangeNodeAuthTokenCommandHandler : IApplicationCommandHandler<ChangeNodeAuthTokenCommand,
    ChangeNodeAuthTokenCommandResult>
{
    public ChangeNodeAuthTokenCommandHandler()
    {
    }

    public async Task<ChangeNodeAuthTokenCommandResult> HandleAsync(ChangeNodeAuthTokenCommand command)
    {
        return new();
    }
}