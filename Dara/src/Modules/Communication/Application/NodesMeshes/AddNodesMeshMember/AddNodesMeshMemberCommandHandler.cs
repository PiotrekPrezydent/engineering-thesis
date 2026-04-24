using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Nodes;
using Dara.Modules.Communication.Domain.NodesMeshes;

namespace Dara.Modules.Communication.Application.NodesMeshes.AddNodesMeshMember;

public class AddNodesMeshMemberCommandHandler : IApplicationCommandHandler<AddNodesMeshMemberCommand, AddNodesMeshMemberCommandResult>
{
    private readonly INodesMeshesRepository _nodesMeshesReposiotory;
    private readonly INodesReposiotory _nodesReposiotory;
    
    public AddNodesMeshMemberCommandHandler(INodesMeshesRepository nodesMeshesReposiotory, INodesReposiotory nodesReposiotory)
    {
        _nodesMeshesReposiotory = nodesMeshesReposiotory;
        _nodesReposiotory = nodesReposiotory;
    }

    public async Task<AddNodesMeshMemberCommandResult> HandleAsync(AddNodesMeshMemberCommand memberCommand)
    {
        Node node = await _nodesReposiotory.GetByGuidAsync(memberCommand.NodeId);
        
        NodesMesh mesh = await _nodesMeshesReposiotory.GetByGuidAsync(memberCommand.MeshId);
        
        return new();
    }
}