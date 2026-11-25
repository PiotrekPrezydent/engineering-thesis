using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePlugin;

public record DisablePluginCommand(Guid RequesterId, Guid PluginId) : ICommand;
