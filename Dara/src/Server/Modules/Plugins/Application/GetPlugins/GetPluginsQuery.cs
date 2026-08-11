using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Plugins.Application.GetPlugins;

public record GetPluginsQuery(Guid OwnerId) : IQuery<List<PluginDto>>;