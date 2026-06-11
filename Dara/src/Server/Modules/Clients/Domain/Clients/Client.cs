using Dara.BuildingBlocks.Domain.Models;
using Dara.Server.Modules.Clients.Domain.Clients.Events;
using Dara.Server.Modules.Clients.Domain.Clients.Rules;

namespace Dara.Server.Modules.Clients.Domain.Clients;

public class Client : Entity, IAggregateRoot
{
    
    public string Name { get; set; }
    
    public bool IsActive { get; set; }
    
    // public List<ClientId> ClientSupervisors;

    internal Client(ClientId clientId, string name) : base(clientId)
    {
        
        IsActive = false;

        //ClientSupervisors = new();

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

    public void ChangeName(string name)
    {
        CheckRule(new ClientSessionMustBeStartedToPreformActionsRule(this, nameof(ChangeName)));
        CheckRule(new ClientNameCannotBeEmptyRule(name));
        
        Name = name;
        
        AddDomainEvent(new ClientChangedNameDomainEvent());
    }
    
    // public void AddSupervisor(ClientId supervisor)
    // {
    //     CheckRule(new ClientSessionMustBeStartedToPreformActionsRule(this, nameof(AddSupervisor)));
    //     
    //     ClientSupervisors.Add(supervisor);
    //     
    //     AddDomainEvent(new ClientAddedSupervisorDomainEvent());
    // }
    //
    // public void RemoveSupervisor(ClientId supervisor)
    // {
    //     CheckRule(new ClientSessionMustBeStartedToPreformActionsRule(this, nameof(RemoveSupervisor)));
    //     
    //     ClientSupervisors.Remove(supervisor);
    //     
    //     AddDomainEvent(new ClientRemovedSupervisorDomainEvent());
    // }
    
    public void EndSession()
    {
        CheckRule(new ClientSessionMustBeStartedToPreformActionsRule(this, nameof(EndSession)));
        
        IsActive = false;
        
        AddDomainEvent(new ClientEndedSessionDomainEvent());
    }

    public override IEntityId Id { get; }
}