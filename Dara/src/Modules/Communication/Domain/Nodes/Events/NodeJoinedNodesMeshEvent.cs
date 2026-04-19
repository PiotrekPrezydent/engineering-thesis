using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.NodesMeshs;

namespace Dara.Modules.Communication.Domain.Nodes.Events;

class NodeJoinedNodesMeshEvent : IDomainEvent
{
    public Node Context { get; }
    
    public NodesMesh JoinedNodesMesh { get; }
    
    public NodeJoinedNodesMeshEvent(Node context, NodesMesh joinedNodesMesh)
    {
        Context = context;
        JoinedNodesMesh = joinedNodesMesh;
    }
}