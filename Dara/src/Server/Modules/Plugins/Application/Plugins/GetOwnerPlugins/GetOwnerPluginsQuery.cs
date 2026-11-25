using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;

public record GetOwnerPluginsQuery(Guid OwnerId) : IQuery<List<OwnerPluginDto>>;
