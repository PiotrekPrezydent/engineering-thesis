using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.GetMyAssignments;

public class GetMyAssignmentsQueryHandler : IQueryHandler<GetMyAssignmentsQuery, List<RoleAssignmentDto>>
{
    private readonly IReadModel _readModel;

    public GetMyAssignmentsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<RoleAssignmentDto>> HandleAsync(GetMyAssignmentsQuery query)
    {
        var held = await SubjectAccess.ReadAsync(_readModel, query.SenderId);
        return held
            .OrderBy(h => h.Assignment.ScopeCode, StringComparer.Ordinal)
            .ThenBy(h => h.Assignment.RoleCode, StringComparer.Ordinal)
            .Select(h => new RoleAssignmentDto(h.Assignment.SubjectId, h.Assignment.RoleCode, h.Assignment.ScopeCode,
                h.Assignment.ScopeId, h.PermissionCodes.ToList()))
            .ToList();
    }
}
