using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Domain.NodesMeshs.Events;

class NodesMeshMemberAddedEvent : IDomainEvent
{
    public NodesMesh ChangedNodesMesh { get; }
    
    public Node AddedMember { get; }
    
    public NodesMeshMemberAddedEvent(NodesMesh changedNodesMesh, Node addedMember)
    {
        ChangedNodesMesh = changedNodesMesh;
        AddedMember = addedMember;
    }
}