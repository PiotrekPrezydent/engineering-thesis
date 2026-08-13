using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins.Rules;

public class OnlyActiveFunctionCanBeDeactivated : IBuisnessRule
{
    private bool _isActive;

    public OnlyActiveFunctionCanBeDeactivated(bool isActive)
    {
        _isActive = isActive;
    }
    
    public string Message => nameof(OnlyActiveFunctionCanBeDeactivated);
    public bool IsBroken()
    {
        return !_isActive;
    }
}