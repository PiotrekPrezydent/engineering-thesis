using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations;

public class PluginConfiguration : Entity, IAggregateRoot
{
    public PluginConfigurationId Id { get; private set; }
    
    public PluginOwnerId OwnerId { get; private set; }

    public PluginId PluginId { get; private set; }

    public IReadOnlyList<PluginFunctionConfiguration> FunctionConfigurations => _functionConfigurations.AsReadOnly();
    
    private List<PluginFunctionConfiguration> _functionConfigurations;
    
    public bool IsEnabled { get; private set; }
    
    // grows with every change, so consumers of the published state can drop older states
    public int Version { get; private set; }

    private  PluginConfiguration()
    {}

    private PluginConfiguration(PluginOwnerId ownerId, PluginId pluginId, IReadOnlyList<PluginFunctionId> functionIds)
    {
        Id = new PluginConfigurationId(Guid.NewGuid());
        
        OwnerId = ownerId;
        PluginId = pluginId;
        _functionConfigurations = functionIds.Select(PluginFunctionConfiguration.Create).ToList();
        IsEnabled = true;
        Version = 1;
    }

    // a new plugin starts enabled with every function enabled
    public static PluginConfiguration Create(PluginOwnerId ownerId, PluginId pluginId, IReadOnlyList<PluginFunctionId> functionIds)
    {
        return new PluginConfiguration(ownerId, pluginId, functionIds);
    }

    public void EnablePlugin(PluginOwnerId requesterId)
    {
        CheckRule(new OnlyPluginOwnerCanConfigurePluginRule(OwnerId, requesterId));
        CheckRule(new OnlyNonActivePluginCanBeEnabledRule(IsEnabled));
        
        IsEnabled = true;
        Version++;
        
        AddDomainEvent(new PluginEnabledDomainEvent(Id, OwnerId, PluginId, Version));
    }

    public void DisablePlugin(PluginOwnerId requesterId)
    {
        CheckRule(new OnlyPluginOwnerCanConfigurePluginRule(OwnerId, requesterId));
        CheckRule(new OnlyActivePluginCanBeDisabledRule(IsEnabled));
        
        IsEnabled = false;
        Version++;
        
        AddDomainEvent(new PluginDisabledDomainEvent(Id, OwnerId, PluginId, Version));
    }

    public void EnableFunction(PluginOwnerId requesterId, PluginFunctionId functionId)
    {
        CheckRule(new OnlyPluginOwnerCanConfigurePluginRule(OwnerId, requesterId));
        var function = GetFunction(functionId);
        
        function.Enable();
        Version++;

        AddDomainEvent(new PluginFunctionEnabledDomainEvent(Id, OwnerId, PluginId, functionId, Version));
    }

    public void DisableFunction(PluginOwnerId requesterId, PluginFunctionId functionId)
    {
        CheckRule(new OnlyPluginOwnerCanConfigurePluginRule(OwnerId, requesterId));
        var function = GetFunction(functionId);
        
        function.Disable();
        Version++;

        AddDomainEvent(new PluginFunctionDisabledDomainEvent(Id, OwnerId, PluginId, functionId, Version));
    }

    // the configuration goes away with its plugin, the last version says that no function is left
    public void Remove()
    {
        Version++;

        AddDomainEvent(new PluginConfigurationRemovedDomainEvent(Id, OwnerId, PluginId, Version));
    }

    // functions the agent may use now, none while the plugin is disabled
    public IReadOnlyList<PluginFunctionId> GetAvailableFunctionIds()
    {
        if (!IsEnabled)
            return [];

        return _functionConfigurations.Where(e => e.IsEnabled).Select(e => e.FunctionId).ToList();
    }

    private PluginFunctionConfiguration GetFunction(PluginFunctionId functionId)
    {
        CheckRule(new FunctionMustBelongToPluginRule(_functionConfigurations, functionId));

        return _functionConfigurations.Single(e => e.FunctionId == functionId);
    }
}