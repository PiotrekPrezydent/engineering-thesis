using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Infrastructure;

public class NodesRepository : INodesReposiotory
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly List<Node> _nodes;

    public NodesRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _nodes = new();
    }
    public Task<Node> GetByGuidAsync(Guid guid)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Node>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Node aggregateRoot)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Node aggregateRoot)
    {
        throw new NotImplementedException();
    }

    public Task<Node> GetByNodeClientOwnerAsync(Client nodeClientOwner)
    {
        throw new NotImplementedException();
    }

    public Task<Node> GetByNameAsync(NodeName nodeName)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(Node node)
    {
        throw new NotImplementedException();
    }
}