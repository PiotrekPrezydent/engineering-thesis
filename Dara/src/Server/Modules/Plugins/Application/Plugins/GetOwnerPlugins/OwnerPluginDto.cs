using Dara.Shared.Contracts.Functions;

namespace Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;

public record OwnerPluginDto(Guid PluginId, string Name, string? Description, string Version, string Sha256, bool IsEnabled, List<OwnerPluginFunctionDto> Functions);

public record OwnerPluginFunctionDto(Guid FunctionId, bool IsEnabled, FunctionDescription Description);
