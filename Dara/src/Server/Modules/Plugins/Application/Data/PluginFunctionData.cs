using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Application.Data;

public record PluginFunctionData(string Name, string Description, string ReturnTypeName)
{
    internal static PluginFunctionData FromFunction(PluginFunction function) =>
        new PluginFunctionData(function.Name, function.Description, function.ReturnType);
    
}