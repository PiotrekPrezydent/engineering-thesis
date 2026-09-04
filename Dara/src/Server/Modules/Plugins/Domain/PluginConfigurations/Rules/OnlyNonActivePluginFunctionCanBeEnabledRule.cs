using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;

public class OnlyNonActivePluginFunctionCanBeEnabledRule : IBuisnessRule
{
    private readonly bool _isEnabled;

    public OnlyNonActivePluginFunctionCanBeEnabledRule(bool isEnabled)
    {
        _isEnabled = isEnabled;
    }

    public string Message => "OnlyNonActivePluginFunctionCanBeEnabledRule";
    public bool IsBroken()
    {
        return _isEnabled;
    }
}