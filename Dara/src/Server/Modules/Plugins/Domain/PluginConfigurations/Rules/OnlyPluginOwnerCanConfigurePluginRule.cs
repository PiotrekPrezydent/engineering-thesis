using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;

public class OnlyPluginOwnerCanConfigurePluginRule : IBuisnessRule
{
    private readonly PluginOwnerId _ownerId;
    private readonly PluginOwnerId _requesterId;

    public OnlyPluginOwnerCanConfigurePluginRule(PluginOwnerId ownerId, PluginOwnerId requesterId)
    {
        _ownerId = ownerId;
        _requesterId = requesterId;
    }

    public string Message => "Only the plugin owner can change its configuration";
    public bool IsBroken()
    {
        return _ownerId != _requesterId;
    }
}
