using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.BuildingBlocks.Domain.Exceptions;
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
    public async Task<Node> GetByGuidAsync(Guid guid)
    {
        var node = _nodes.FirstOrDefault(x => x.Id == guid, null);
        
        if (node == null)
            throw new EntityNotFoundInRepositoryException<NodesRepository, Node>(this, guid);

        return node;
    }

    public async Task<IEnumerable<Node>> GetAllAsync()
    {
        return _nodes.AsEnumerable();
    }

    public async Task AddAsync(Node aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);

        _nodes.Add(aggregateRoot);
    }

    public async Task RemoveAsync(Node aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        await _domainEventDispatcher.DispatchAsync(new EntityRemovedEvent<Node>(this, aggregateRoot));
        
        _nodes.Remove(aggregateRoot);
    }

    public async Task<Node> GetByNodeClientOwnerAsync(Client nodeClientOwner)
    {
        var node = _nodes.FirstOrDefault(x => x.NodeOwner == nodeClientOwner, null);
        
        if (node == null)
            throw new EntityNotFoundInRepositoryException<NodesRepository, Node>(this, nodeClientOwner);

        return node;
    }

    public async Task<Node> GetByNameAsync(NodeName nodeName)
    {
        var node = _nodes.FirstOrDefault(x => x.Name == nodeName, null);
        
        if (node == null)
            throw new EntityNotFoundInRepositoryException<NodesRepository, Node>(this, nodeName);

        return node;
    }

    public async Task SaveAsync(Node node)
    {
        var nd = _nodes.FirstOrDefault(e => e == node, null);
        
        if(nd == null)
            throw new EntityNotFoundInRepositoryException<NodesRepository,Node>(this, node);
        
        foreach (var domainEvent in nd.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
    }
}