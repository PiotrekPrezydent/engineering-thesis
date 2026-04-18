using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Domain.Groups.Events;

class GroupMemberRemovedEvent : IDomainEvent
{
    public Group ChangedGroup { get; }
    
    public Client RemovedMember { get; }
    
    public GroupMemberRemovedEvent(Group changedGroup, Client removedMember)
    {
        ChangedGroup = changedGroup;
        RemovedMember= removedMember;
    }
}