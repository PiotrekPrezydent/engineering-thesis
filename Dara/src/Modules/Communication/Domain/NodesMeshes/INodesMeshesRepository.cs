using Dara.BuildingBlocks.Domain;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Domain.NodesMeshs;

public interface INodesMeshesRepository : IRepository<NodesMesh>
{
    public Task<NodesMesh> GetByNameAsync(NodesMeshName nodesMeshName);
    
    public Task<IEnumerable<Node>> GetMembersAsync(NodesMesh nodesMesh);
    
    public Task SaveAsync(NodesMesh nodesMesh);
}