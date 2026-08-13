using Dara.Server.Modules.Plugins.Application.Data;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Application.GetPlugins;

public record PluginDto(Guid PluginId, PluginData PluginData, IReadOnlyList<PluginFunctionDto> Functions)
{
    internal static PluginDto FromPlugin(Plugin plugin) => new PluginDto(
        plugin.Id,
        PluginData.FromPlugin(plugin),
        plugin.Functions.Select(PluginFunctionDto.FromFunction).ToList());
}

public record PluginFunctionDto(
    Guid FunctionId,
    PluginFunctionData FunctionData,
    IReadOnlyList<PluginFunctionParameterDto> Parameters)
{
    internal static PluginFunctionDto FromFunction(PluginFunction function) => new PluginFunctionDto(
        function.Id, 
        PluginFunctionData.FromFunction(function), 
        function.Parameters.Select(PluginFunctionParameterDto.FromParameter).ToList());
}

public record PluginFunctionParameterDto(PluginFunctionParameterData ParameterData)
{
    internal static PluginFunctionParameterDto FromParameter(PluginFunctionParameter parameter) => new PluginFunctionParameterDto(PluginFunctionParameterData.FromParameter(parameter));
}