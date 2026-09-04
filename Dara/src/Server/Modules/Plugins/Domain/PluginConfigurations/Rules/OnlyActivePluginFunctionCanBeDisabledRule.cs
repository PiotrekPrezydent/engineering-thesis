using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;

public class OnlyActivePluginFunctionCanBeDisabledRule : IBuisnessRule
{
    private readonly bool _isEnabled;

    public OnlyActivePluginFunctionCanBeDisabledRule(bool isEnabled)
    {
        _isEnabled = isEnabled;
    }

    public string Message => "Only active plugin function can be disabled";
    public bool IsBroken()
    {
        return !_isEnabled;
    }
}