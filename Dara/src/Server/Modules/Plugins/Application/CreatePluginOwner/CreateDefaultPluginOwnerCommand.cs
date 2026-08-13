using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.CreatePluginOwner;

public record CreateDefaultPluginOwnerCommand(Guid PluginOwnerId) : ICommand;