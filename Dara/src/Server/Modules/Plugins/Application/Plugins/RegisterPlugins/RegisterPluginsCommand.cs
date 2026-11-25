using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Shared.Contracts.Plugins;

namespace Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;

// the full set of plugins a client has now, sent after every connection
public record RegisterPluginsCommand(Guid OwnerId, IReadOnlyList<PluginManifest> Plugins) : ICommand;
