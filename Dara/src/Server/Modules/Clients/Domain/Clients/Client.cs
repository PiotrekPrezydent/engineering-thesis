using Dara.BuildingBlocks.Domain;
using Dara.Server.Modules.Clients.Domain.Clients.Events;
using Dara.Server.Modules.Clients.Domain.Clients.Rules;

namespace Dara.Server.Modules.Clients.Domain.Clients;

public class Client : Entity<ClientId>, IAggregateRoot
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    
    internal Client(ClientId clientId, string name) : base(clientId)
    {
        IsActive = false;
    
        Name = name;
    }

    public static Client Create(ClientId clientId, string name)
    {
        return new(clientId, name);
    }

    public void StartSession()
    {
        CheckRule(new ClientSessionCannotBeStartedTwiceRule(IsActive));
        
        IsActive = true;
        
        AddDomainEvent(new ClientStartedSessionDomainEvent());
    }
    
    public void EndSession()
    {
        CheckRule(new ClientSessionMustBeStartedToPreformActionsRule(IsActive, nameof(EndSession)));
        
        IsActive = false;
        
        AddDomainEvent(new ClientEndedSessionDomainEvent());
    }

    public void ChangeName(string name)
    {
        CheckRule(new ClientSessionMustBeStartedToPreformActionsRule(IsActive, nameof(ChangeName)));
        CheckRule(new ClientNameCannotBeEmptyRule(name));
        
        Name = name;
        
        AddDomainEvent(new ClientChangedNameDomainEvent());
    }
}