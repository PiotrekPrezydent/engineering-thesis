using System.Collections.Immutable;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Application.GetPlugins;

public class GetPluginsQueryHandler : IQueryHandler<GetPluginsQuery,List<PluginDto>>
{
    private readonly IReadModel _readModel;

    public GetPluginsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<PluginDto>> HandleAsync(GetPluginsQuery query)
    {
        var userPlugins = await _readModel.Query<PluginOwner>()
            .Include(p=>p.Plugins)
            .FirstAsync(e => e.Id.Match(query.OwnerId));

        List<PluginDto> result = new();
        foreach (var plugin in userPlugins.Plugins)
        {
            List<PluginFunctionDto> functions = new();
            foreach (var function in plugin.Functions)
            {
                functions.Add(new PluginFunctionDto(function.Name,
                    function.Description,
                    function.ReturnType,
                    function.Parameters
                        .Select(e => new PluginFunctionParameterDto(e.Name, e.Description, e.Type))
                        .ToImmutableArray()
                    ));
            }
            result.Add(new PluginDto(plugin.Name,plugin.Description,functions.ToImmutableArray()));
        }

        return result;
    }
}