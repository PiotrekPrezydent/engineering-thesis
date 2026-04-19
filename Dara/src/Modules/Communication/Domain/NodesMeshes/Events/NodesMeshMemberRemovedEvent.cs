using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Domain.NodesMeshes.Events;

class NodesMeshMemberRemovedEvent : IDomainEvent
{
    public NodesMesh ChangedNodesMesh { get; }
    
    public Node RemovedMember { get; }
    
    public NodesMeshMemberRemovedEvent(NodesMesh changedNodesMesh, Node removedMember)
    {
        ChangedNodesMesh = changedNodesMesh;
        RemovedMember= removedMember;
    }
}