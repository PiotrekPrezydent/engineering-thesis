using Dara.Server.Apps.API.Common.Errors;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

namespace Dara.Server.Apps.API.Modules.Plugins;

// rules of plugins that are no validation: the owner configures (403), a repeated enable, disable, add or remove (409)
public static class PluginsProblems
{
    public static IResult? From(IBuisnessRule rule)
    {
        return rule switch
        {
            OnlyPluginOwnerCanConfigurePluginRule => DomainProblems.Forbidden(rule),
            PluginCannotBeAddedTwiceRule or NonActualPluginCannotBeRemovedRule
                or OnlyActivePluginCanBeDisabledRule or OnlyNonActivePluginCanBeEnabledRule
                or OnlyActivePluginFunctionCanBeDisabledRule or OnlyNonActivePluginFunctionCanBeEnabledRule => DomainProblems.Conflict(rule),
            _ => null
        };
    }
}
