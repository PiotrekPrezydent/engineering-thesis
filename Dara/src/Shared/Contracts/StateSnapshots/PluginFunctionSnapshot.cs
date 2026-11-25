using Dara.Shared.Contracts.Functions;

namespace Dara.Shared.Contracts.StateSnapshots;

public record PluginFunctionSnapshot(Guid FunctionId, bool IsEnabled, FunctionDescription Description);
