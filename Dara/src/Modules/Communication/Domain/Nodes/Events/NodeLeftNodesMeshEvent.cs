using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.NodesMeshes;

namespace Dara.Modules.Communication.Domain.Nodes.Events;

class NodeLeftNodesMeshEvent : IDomainEvent
{
    public Node Context { get; }
    
    public NodesMesh LeavedNodesMesh { get; }
    
    public NodeLeftNodesMeshEvent(Node context, NodesMesh leavedNodesMesh)
    {
        Context = context;
        LeavedNodesMesh = leavedNodesMesh;
    }
}