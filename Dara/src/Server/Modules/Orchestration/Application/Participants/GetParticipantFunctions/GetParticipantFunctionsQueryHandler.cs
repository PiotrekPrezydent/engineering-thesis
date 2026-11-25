using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;

public class GetParticipantFunctionsQueryHandler : IQueryHandler<GetParticipantFunctionsQuery, List<ParticipantFunctionDto>>
{
    private readonly IReadModel _readModel;

    public GetParticipantFunctionsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<ParticipantFunctionDto>> HandleAsync(GetParticipantFunctionsQuery query)
    {
        var functions = await _readModel
            .Query<Function>()
            .Where(f => f.ParticipantId.Match(query.ParticipantId))
            .ToListAsync();

        return functions
            .OrderBy(f => f.Name)
            .Select(f => new ParticipantFunctionDto(f.Id.Value, f.PluginId.Value, f.Name, f.Description))
            .ToList();
    }
}
