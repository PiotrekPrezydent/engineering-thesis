using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Domain.Models.Abstraction;
using Dara.Server.Modules.Identity.Domain.Clients.Events;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;

namespace Dara.Server.Modules.Identity.Domain.Clients;

public class Client : Entity, IAggregateRoot
{
    public ClientId Id { get; }
    
    public string Name { get; set; }
    
    public List<ClientPlugin> Plugins { get; set; }

    internal Client(ClientId id, string name, List<ClientPlugin> plugins)
    {
        Id = id;

        Name = name;
        Plugins = plugins;
        
        AddDomainEvent(new ClientStartedSessionDomainEvent());
    }

    public static Client StartSession(ClientId id, string name, List<ClientPlugin> plugins)
    {
        return new(id, name, plugins);
    }

    public void ChangeName(string name)
    {
        CheckRule(new ClientNameCannotBeEmptyRule(name));
        
        Name = name;
        AddDomainEvent(new ClientChangedNameDomainEvent());
    }
    

    public void AddPlugin(ClientPlugin clientPlugin)
    {
        CheckRule(new ClientPluginCannotBeAddedTwiceRule(Plugins, clientPlugin));
        
        Plugins.Add(clientPlugin);
        AddDomainEvent(new ClientAddedPluginDomainEvent());
    }

    public void RemovePlugin(ClientPlugin clientPlugin)
    {
        CheckRule(new OnlyUsedExistingPluginCanBeRemovedRule(Plugins, clientPlugin));
        
        Plugins.Remove(clientPlugin);
        AddDomainEvent(new ClientRemovedPluginDomainEvent());
    }
    
    public void EndSession()
    {
        CheckRule(new OnlyClientWithSessionCanStopClientSessionRule(Id == null));
        
        AddDomainEvent(new ClientEndedSessionDomainEvent());
    }
}