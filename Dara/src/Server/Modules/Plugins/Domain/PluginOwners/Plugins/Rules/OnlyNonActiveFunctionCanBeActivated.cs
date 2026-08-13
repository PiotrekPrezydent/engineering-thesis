using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins.Rules;

public class OnlyNonActiveFunctionCanBeActivated : IBuisnessRule
{
    private bool _isActive;

    public OnlyNonActiveFunctionCanBeActivated(bool isActive)
    {
        _isActive = isActive;
    }
    
    public string Message => nameof(OnlyNonActiveFunctionCanBeActivated);
    public bool IsBroken()
    {
        return _isActive;
    }
}