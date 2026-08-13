using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Application.Data;

public record PluginFunctionParameterData(string Name, string Description, string TypeName)
{
    internal static PluginFunctionParameterData FromParameter(PluginFunctionParameter parameter) => new PluginFunctionParameterData(parameter.Name, parameter.Description, parameter.Type);
}