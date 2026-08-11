using System.Collections.Immutable;

namespace Dara.Server.Modules.Plugins.Application.GetPlugins;

public record PluginDto(string Name, string Description, ImmutableArray<PluginFunctionDto> Functions);

public record PluginFunctionDto(string Name, string Description, string ReturnType, ImmutableArray<PluginFunctionParameterDto> Parameters);

public record PluginFunctionParameterDto(string Name, string Description, string Type);