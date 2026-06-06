using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

public class OnlyUsedExistingPluginCanBeRemovedRule : IDomainRule
{
    List<ClientPlugin>  _clientPlugins;
    ClientPlugin _newPlugin;
    
    internal OnlyUsedExistingPluginCanBeRemovedRule(List<ClientPlugin> clientPlugins, ClientPlugin newPlugin)
    {
        _clientPlugins = clientPlugins;
        _newPlugin = newPlugin;
    }

    public string Message => $"{nameof(OnlyUsedExistingPluginCanBeRemovedRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return !_clientPlugins.Contains(_newPlugin);
    }

}