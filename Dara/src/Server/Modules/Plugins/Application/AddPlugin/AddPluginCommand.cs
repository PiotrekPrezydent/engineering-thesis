using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Application.Data;

namespace Dara.Server.Modules.Plugins.Application.AddPlugin;

public record AddPluginCommand(Guid PluginOwnerId, PluginDescriptor PluginDescriptor) : ICommand;