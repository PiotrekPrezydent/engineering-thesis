using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.BuildingBlocks.Domain.Exceptions;
using Dara.Modules.Communication.Domain.Nodes;
using Dara.Modules.Communication.Domain.NodesMeshes;

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
    
    public async Task<NodesMesh> GetByGuidAsync(Guid guid)
    {
        var nm = _nodesMeshes.FirstOrDefault(e => e.Id == guid, null);
        
        if(nm == null)
            throw new EntityNotFoundInRepositoryException<NodesMeshesRepository,NodesMesh>(this, guid);
        
        return nm;
    }

    public async Task<IEnumerable<NodesMesh>> GetAllAsync()
    {
        return _nodesMeshes.AsEnumerable();
    }

    public async Task AddAsync(NodesMesh aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);

        _nodesMeshes.Add(aggregateRoot);
    }

    public async Task RemoveAsync(NodesMesh aggregateRoot)
    {
        foreach (var domainEvent in aggregateRoot.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        await _domainEventDispatcher.DispatchAsync(new EntityRemovedEvent<NodesMesh>(this, aggregateRoot));
        
        _nodesMeshes.Remove(aggregateRoot);
    }

    public async Task<NodesMesh> GetByNameAsync(NodesMeshName nodesMeshName)
    {
        var nm = _nodesMeshes.FirstOrDefault(e => e.Name == nodesMeshName, null);
        
        if(nm == null)
            throw new EntityNotFoundInRepositoryException<NodesMeshesRepository,NodesMesh>(this, nodesMeshName);
        
        return nm;
    }

    public async Task<IEnumerable<Node>> GetMembersAsync(NodesMesh nodesMesh)
    {
        var nm = _nodesMeshes.FirstOrDefault(e => e == nodesMesh, null);
        
        if(nm == null)
            throw new EntityNotFoundInRepositoryException<NodesMeshesRepository,NodesMesh>(this, nodesMesh);

        return nm.Members.AsEnumerable();
    }

    public async Task SaveAsync(NodesMesh nodesMesh)
    {
        var nm = _nodesMeshes.FirstOrDefault(e => e == nodesMesh, null);
        
        if(nm == null)
            throw new EntityNotFoundInRepositoryException<NodesMeshesRepository,NodesMesh>(this, nodesMesh);
        
        foreach (var domainEvent in nm.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
    }
}