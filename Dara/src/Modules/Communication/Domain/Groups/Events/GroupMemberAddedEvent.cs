using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Domain.Groups.Events;

class GroupMemberAddedEvent : IDomainEvent
{
    public Group ChangedGroup { get; }
    
    public Client AddedMember { get; }
    
    public GroupMemberAddedEvent(Group changedGroup, Client addedMember)
    {
        ChangedGroup = changedGroup;
        AddedMember = addedMember;
    }
}