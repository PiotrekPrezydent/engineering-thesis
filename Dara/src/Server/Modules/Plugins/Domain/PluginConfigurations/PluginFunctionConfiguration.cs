using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations;

// events of a function change are raised by the configuration root
public class PluginFunctionConfiguration : Entity
{
    public PluginFunctionConfigurationId Id { get; private set; }
    
    public PluginConfiguration PluginConfiguration { get; private set; }
    public PluginConfigurationId PluginConfigurationId { get; private set; }
    
    public PluginFunctionId FunctionId { get; private set; }
    
    public bool IsEnabled { get; private set; }

    private PluginFunctionConfiguration()
    {
    }

    private PluginFunctionConfiguration(PluginFunctionId functionId, bool isEnabled)
    {
        Id = new PluginFunctionConfigurationId(Guid.NewGuid());
        
        FunctionId = functionId;
        IsEnabled = isEnabled;
    }

    internal static PluginFunctionConfiguration Create(PluginFunctionId functionId)
    {
        return new PluginFunctionConfiguration(functionId, true);
    }
    
    internal void Enable()
    {
        CheckRule(new OnlyNonActivePluginFunctionCanBeEnabledRule(IsEnabled));
        
        IsEnabled = true;
    }

    internal void Disable()
    {
        CheckRule(new OnlyActivePluginFunctionCanBeDisabledRule(IsEnabled));
        
        IsEnabled = false;
    }
 
}