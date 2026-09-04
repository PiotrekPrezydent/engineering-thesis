using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;

public class OnlyNonActivePluginCanBeEnabledRule : IBuisnessRule
{
    private readonly bool _isEnabled;

    public OnlyNonActivePluginCanBeEnabledRule(bool isEnabled)
    {
        _isEnabled = isEnabled;
    }

    public string Message => "Only non-active plugin can be enabled";
    public bool IsBroken()
    {
        return _isEnabled;
    }
}