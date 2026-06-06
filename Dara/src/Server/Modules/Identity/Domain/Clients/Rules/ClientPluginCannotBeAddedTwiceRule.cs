using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

public class ClientPluginCannotBeAddedTwiceRule : IDomainRule
{
    List<ClientPlugin>  _clientPlugins;
    ClientPlugin _newPlugin;
    
    internal ClientPluginCannotBeAddedTwiceRule(List<ClientPlugin> clientPlugins, ClientPlugin newPlugin)
    {
        _clientPlugins = clientPlugins;
        _newPlugin = newPlugin;
    }

    public string Message => $"{nameof(ClientPluginCannotBeAddedTwiceRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return _clientPlugins.Contains(_newPlugin);
    }

}