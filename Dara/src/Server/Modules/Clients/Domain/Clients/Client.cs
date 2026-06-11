using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Clients.Domain.Clients.Events;
using Dara.Server.Modules.Clients.Domain.Clients.Rules;

namespace Dara.Server.Modules.Clients.Domain.Clients;

public class Client : Entity<ClientId>, IAggregateRoot
{
    public string Name { get; private set; }
    public bool IsOnline { get; private set; }

    public IReadOnlyList<ClientId> ClientSupervisors => _clientSupervisors.AsReadOnly();
    private readonly List<ClientId> _clientSupervisors;

    private Client() { }
    
    internal Client(ClientId clientId, string name)
    {
        Id = clientId;
        IsOnline = false;
        _clientSupervisors = new();
        Name = name;
        
        AddDomainEvent(new ClientCreatedDomainEvent(Id));
    }

    public static Client Create(ClientId clientId, string name)
    {
        return new(clientId, name);
    }

    public void Delete()
    {
        AddDomainEvent(new ClientDeletedDomainEvent(Id));
    }
    
    public void MarkAsOnline()
    {
        CheckRule(new OnlyOfflineClientCanBeMarkedAsOnlineRule(IsOnline));
        
        IsOnline = true;
        AddDomainEvent(new ClientWentOnlineDomainEvent(Id));
    }
    
    public void MarkAsOffline()
    {
        CheckRule(new OnlyOnlineClientCanBeMarkedAsOfflineRule(IsOnline));
        
        IsOnline = false;
        AddDomainEvent(new ClientWentOfflineDomainEvent(Id));
    }
    
    public void ChangeName(string name)
    {
        CheckRule(new ClientNameCannotBeEmptyRule(name));
        
        Name = name;
        AddDomainEvent(new ClientNameChangedDomainEvent(Id, Name));
    }

    public void AddSupervisor(ClientId supervisor)
    {
        CheckRule(new ClientSupervisorCannotBeAddedTwiceRule(ClientSupervisors, supervisor));
        
        _clientSupervisors.Add(supervisor);
        AddDomainEvent(new ClientSupervisorAddedDomainEvent(Id, supervisor));
    }

    public void RemoveSupervisor(ClientId supervisor)
    {
        CheckRule(new OnlyActualClientSupervisorCanBeRemovedRule(ClientSupervisors, supervisor));
        
        _clientSupervisors.Remove(supervisor);
        AddDomainEvent(new ClientSupervisorRemovedDomainEvent(Id, supervisor));
    }
    
}