using System.Collections.Immutable;

namespace Dara.Shared.Contracts;

public record PluginData(string Name, string Description, ImmutableArray<PluginFunctionData> Functions);

public record PluginFunctionData(string Name, string Description, string ReturnType, ImmutableArray<PluginFunctionParameterData> Parameters);

public record PluginFunctionParameterData(string Name, string Description, string Type);