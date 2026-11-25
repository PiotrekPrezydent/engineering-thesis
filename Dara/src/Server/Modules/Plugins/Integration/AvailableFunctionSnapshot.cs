using Dara.Shared.Contracts.Functions;

namespace Dara.Server.Modules.Plugins.Integration;

// an enabled function of an enabled plugin, the id is the plugin function id
public record AvailableFunctionSnapshot(Guid FunctionId, FunctionDescription Description);
