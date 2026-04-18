using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Groups;

namespace Dara.Modules.Communication.Domain.Clients.Events;

class ClientJoinedGroupEvent : IDomainEvent
{
    public Client Context { get; }
    
    public Group JoinedGroup { get; }
    
    public ClientJoinedGroupEvent(Client context, Group joinedGroup)
    {
        Context = context;
        JoinedGroup = joinedGroup;
    }
}