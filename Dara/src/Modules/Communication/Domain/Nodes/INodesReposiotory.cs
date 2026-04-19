using Dara.BuildingBlocks.Domain;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Domain.Nodes;

public interface INodesReposiotory : IRepository<Node>
{
    public Task<Node> GetByNodeClientOwnerAsync(Client nodeClientOwner);
    
    public Task<Node> GetByNameAsync(NodeName nodeName);
    
    public Task SaveAsync(Node node);
}