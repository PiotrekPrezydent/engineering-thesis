namespace Dara.Server.Modules.Plugins.Integration;

public record PluginSnapshot(Guid PluginId, string Name, string Description, IReadOnlyList<PluginFunctionSnapshot> Functions);

public record PluginFunctionSnapshot(Guid FunctionId, string Name, string Description, string ReturnTypeName, IReadOnlyList<PluginFunctionParameterSnapshot> Parameters);

public record PluginFunctionParameterSnapshot(string Name, string TypeName, string Description);