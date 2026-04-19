using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;
using Dara.Modules.Communication.Domain.NodesMeshs;

namespace Dara.Modules.Communication.Infrastructure;

public class NodesMeshesRepository : INodesMeshesRepository
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly List<NodesMesh> _nodesMeshes;

    public NodesMeshesRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _nodesMeshes = new();
    }
    
    public Task<NodesMesh> GetByGuidAsync(Guid guid)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<NodesMesh>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(NodesMesh aggregateRoot)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(NodesMesh aggregateRoot)
    {
        throw new NotImplementedException();
    }

    public Task<NodesMesh> GetByNameAsync(NodesMeshName nodesMeshName)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Node>> GetMembersAsync(NodesMesh nodesMesh)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(NodesMesh nodesMesh)
    {
        throw new NotImplementedException();
    }
}