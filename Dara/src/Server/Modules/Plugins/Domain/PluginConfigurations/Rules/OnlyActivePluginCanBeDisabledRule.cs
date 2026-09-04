using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;

public class OnlyActivePluginCanBeDisabledRule : IBuisnessRule
{
    private bool _isEnabled;

    public OnlyActivePluginCanBeDisabledRule(bool isEnabled)
    {
        _isEnabled = isEnabled;
    }
    
    public string Message => "Only active plugin can be disabled";
    public bool IsBroken()
    {
        return !_isEnabled;
    }
}