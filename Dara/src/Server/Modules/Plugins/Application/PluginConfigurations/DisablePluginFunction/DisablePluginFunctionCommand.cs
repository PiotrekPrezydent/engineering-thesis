using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePluginFunction;

public record DisablePluginFunctionCommand(Guid RequesterId, Guid PluginId, Guid FunctionId) : ICommand;
