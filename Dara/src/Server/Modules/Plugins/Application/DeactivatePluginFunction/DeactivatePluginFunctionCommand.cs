using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.DeactivatePluginFunction;

public record DeactivatePluginFunctionCommand(Guid PluginOwnerId, Guid PluginId, Guid PluginFunctionId) : ICommand;