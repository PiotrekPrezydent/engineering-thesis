using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Nodes;

public record ChangeNodeNameCommand(Guid NodeId, string NewName) : IApplicationCommand;

public record ChangeNodeNameCommandResult() : IApplicationCommandResult;

public class ChangeNodeNameCommandHandler : IApplicationCommandHandler<ChangeNodeNameCommand, ChangeNodeNameCommandResult>
{
    private readonly INodesReposiotory _nodesReposiotory;
    public ChangeNodeNameCommandHandler(INodesReposiotory nodesReposiotory)
    {
        _nodesReposiotory = nodesReposiotory;
    }

    public async Task<ChangeNodeNameCommandResult> HandleAsync(ChangeNodeNameCommand command)
    {
        NodeName name = new(command.NewName);
        
        var node = await _nodesReposiotory.GetByGuidAsync(command.NodeId);
        
        node.ChangeName(name);
        
        await _nodesReposiotory.SaveAsync(node);
        
        return new();
    }
}