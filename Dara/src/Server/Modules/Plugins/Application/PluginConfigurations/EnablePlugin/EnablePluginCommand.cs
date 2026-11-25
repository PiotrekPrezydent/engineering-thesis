using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePlugin;

public record EnablePluginCommand(Guid RequesterId, Guid PluginId) : ICommand;
