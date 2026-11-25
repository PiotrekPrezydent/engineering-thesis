using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;

public class GetOwnerPluginsQueryHandler : IQueryHandler<GetOwnerPluginsQuery, List<OwnerPluginDto>>
{
    private readonly IReadModel _readModel;

    public GetOwnerPluginsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<OwnerPluginDto>> HandleAsync(GetOwnerPluginsQuery query)
    {
        var plugins = await _readModel
            .Query<Plugin>()
            .Include(p => p.Functions)
            .Where(p => p.OwnerId.Match(query.OwnerId))
            .ToListAsync();

        var configurations = await _readModel
            .Query<PluginConfiguration>()
            .Include(c => c.FunctionConfigurations)
            .Where(c => c.OwnerId.Match(query.OwnerId))
            .ToDictionaryAsync(c => c.PluginId);

        return plugins
            .OrderBy(p => p.Name)
            .Select(p => ToDto(p, configurations[p.Id]))
            .ToList();
    }

    private static OwnerPluginDto ToDto(Plugin plugin, PluginConfiguration configuration)
    {
        var functions = plugin.Functions
            .OrderBy(f => f.Name)
            .Select(f => new OwnerPluginFunctionDto(
                f.Id.Value,
                configuration.FunctionConfigurations.Single(c => c.FunctionId == f.Id).IsEnabled,
                PluginManifestMapper.ToContract(f.GetDescription())))
            .ToList();

        return new OwnerPluginDto(plugin.Id.Value, plugin.Name, plugin.Description, plugin.Version, plugin.Hash.Value, configuration.IsEnabled, functions);
    }
}
