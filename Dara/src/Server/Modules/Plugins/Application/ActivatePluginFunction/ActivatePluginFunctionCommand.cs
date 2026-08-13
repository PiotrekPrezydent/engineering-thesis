using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.ActivatePluginFunction;

public record ActivatePluginFunctionCommand(Guid PluginOwnerId, Guid PluginId, Guid PluginFunctionId) : ICommand;