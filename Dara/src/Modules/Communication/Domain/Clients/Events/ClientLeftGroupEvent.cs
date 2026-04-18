using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Modules.Communication.Domain.Groups;

namespace Dara.Modules.Communication.Domain.Clients.Events;

class ClientLeftGroupEvent : IDomainEvent
{
    public Client Context { get; }
    
    public Group LeavedGroup { get; }
    
    public ClientLeftGroupEvent(Client context, Group leavedGroup)
    {
        Context = context;
        LeavedGroup = leavedGroup;
    }
}