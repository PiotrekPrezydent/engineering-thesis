using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.Modules.Communication.Domain.NodesMeshs.Events;

class NodesMeshBecameEmptyEvent : IDomainEvent
{
    public NodesMesh EmptiedNodesMesh { get; }
    
    public NodesMeshBecameEmptyEvent(NodesMesh emptiedNodesMesh)
    {
        EmptiedNodesMesh = emptiedNodesMesh;
    }
}