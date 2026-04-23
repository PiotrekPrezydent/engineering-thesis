using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Nodes;

public record ChangeNodeAuthTokenCommand(Guid NodeId, string NewAuthToken) : IApplicationCommand;

public record ChangeNodeAuthTokenCommandResult() : IApplicationCommandResult;

public class ChangeNodeAuthTokenCommandHandler : IApplicationCommandHandler<ChangeNodeAuthTokenCommand, ChangeNodeAuthTokenCommandResult>
{
    private readonly INodesReposiotory _nodesReposiotory;
    public ChangeNodeAuthTokenCommandHandler(INodesReposiotory nodesReposiotory)
    {
        _nodesReposiotory = nodesReposiotory;
    }

    public async Task<ChangeNodeAuthTokenCommandResult> HandleAsync(ChangeNodeAuthTokenCommand command)
    {
        NodeAuthToken token = new(command.NewAuthToken);
        
        var node = await _nodesReposiotory.GetByGuidAsync(command.NodeId);
        
        node.ChangeAuthToken(token);

        await _nodesReposiotory.SaveAsync(node);
        
        return new();
    }
}