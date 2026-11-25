using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePluginFunction;

public record EnablePluginFunctionCommand(Guid RequesterId, Guid PluginId, Guid FunctionId) : ICommand;
