using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.Modules.Communication.Domain.NodesMeshes.Events;

class NodesMeshBecameEmptyEvent : IDomainEvent
{
    public NodesMesh EmptiedNodesMesh { get; }
    
    public NodesMeshBecameEmptyEvent(NodesMesh emptiedNodesMesh)
    {
        EmptiedNodesMesh = emptiedNodesMesh;
    }
}