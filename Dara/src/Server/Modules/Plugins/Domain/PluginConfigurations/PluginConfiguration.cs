using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations;

public class PluginConfiguration : Entity, IAggregateRoot
{
    public PluginConfigurationId Id { get; private set; }
    
    public PluginId PluginId { get; private set; }

    public IReadOnlyList<PluginFunctionConfiguration> FunctionsConfigurations => _functionConfigurations.AsReadOnly();
    
    private List<PluginFunctionConfiguration> _functionConfigurations;
    
    public bool IsEnabled { get; private set; }
    
    private  PluginConfiguration()
    {}

    private PluginConfiguration(PluginId pluginId, List<PluginFunctionConfiguration> functionConfigurations, bool isEnabled)
    {
        Id = new PluginConfigurationId(Guid.NewGuid());
        
        PluginId = pluginId;
        _functionConfigurations = functionConfigurations;
        IsEnabled = isEnabled;
    }

    public static PluginConfiguration Create(PluginId pluginId, List<PluginFunctionId> functionConfigurations)
    {
        return new(pluginId, functionConfigurations.Select(PluginFunctionConfiguration.Create).ToList(), true);
    }

    public void EnablePlugin()
    {
        CheckRule(new OnlyNonActivePluginCanBeEnabledRule(IsEnabled));
        
        IsEnabled = true;
        
        AddDomainEvent(new PluginEnabledDomainEvent(PluginId));
    }

    public void DisablePlugin()
    {
        CheckRule(new OnlyActivePluginCanBeDisabledRule(IsEnabled));
        
        IsEnabled = false;
        
        AddDomainEvent(new PluginDisabledDomainEvent(PluginId));
    }

    public void EnableFunction(PluginFunctionId functionId)
    {
        var function = FunctionsConfigurations.Single(e => e.Id == functionId);
        
        function.Enable();
    }

    public void DisableFunction(PluginFunctionId functionId)
    {
        var function = FunctionsConfigurations.Single(e => e.Id == functionId);
        
        function.Disable();
    }
}