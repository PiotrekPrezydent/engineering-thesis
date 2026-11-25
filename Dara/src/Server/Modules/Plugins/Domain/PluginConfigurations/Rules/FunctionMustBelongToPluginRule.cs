using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;

public class FunctionMustBelongToPluginRule : IBuisnessRule
{
    private readonly IReadOnlyList<PluginFunctionConfiguration> _functionConfigurations;
    private readonly PluginFunctionId _functionId;

    public FunctionMustBelongToPluginRule(IReadOnlyList<PluginFunctionConfiguration> functionConfigurations, PluginFunctionId functionId)
    {
        _functionConfigurations = functionConfigurations;
        _functionId = functionId;
    }

    public string Message => "Function must belong to the configured plugin";
    public bool IsBroken()
    {
        return _functionConfigurations.All(e => e.FunctionId != _functionId);
    }
}
