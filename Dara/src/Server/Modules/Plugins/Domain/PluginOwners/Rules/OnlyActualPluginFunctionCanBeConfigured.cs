using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class OnlyActualPluginFunctionCanBeConfigured : IBuisnessRule
{
    private readonly IReadOnlyList<PluginFunction> _currentPluginFunctions;
    private readonly PluginFunctionId _pluginFunctionToConfigure;

    public OnlyActualPluginFunctionCanBeConfigured(IReadOnlyList<PluginFunction> currentPluginFunctions, PluginFunctionId pluginFunctionToConfigure)
    {
        _currentPluginFunctions = currentPluginFunctions;
        _pluginFunctionToConfigure = pluginFunctionToConfigure;
    }

    public string Message => nameof(OnlyActualPluginFunctionCanBeConfigured);
    public bool IsBroken()
    {
        return _currentPluginFunctions.All(e=>e.Id != _pluginFunctionToConfigure);
    }
}