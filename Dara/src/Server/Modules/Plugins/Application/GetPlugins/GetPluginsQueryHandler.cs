using System.Collections.Immutable;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
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
        var owner = await _readModel.Query<PluginOwner>()
            .Include(p=>p.Plugins)
            .ThenInclude(p => p.Functions)
            .FirstAsync(e => e.Id.Match(query.OwnerId));
        
        return owner.Plugins.Select(PluginDto.FromPlugin).ToList();
    }
}