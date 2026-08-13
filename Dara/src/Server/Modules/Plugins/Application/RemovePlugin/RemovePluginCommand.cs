using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.RemovePlugin;

public record RemovePluginCommand(Guid OwnerId, Guid PluginId) : ICommand;