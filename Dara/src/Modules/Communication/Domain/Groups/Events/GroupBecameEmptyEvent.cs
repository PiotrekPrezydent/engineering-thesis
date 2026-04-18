using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.Modules.Communication.Domain.Groups.Events;

class GroupBecameEmptyEvent : IDomainEvent
{
    public Group EmptiedGroup { get; }
    
    public GroupBecameEmptyEvent(Group emptiedGroup)
    {
        EmptiedGroup = emptiedGroup;
    }
}