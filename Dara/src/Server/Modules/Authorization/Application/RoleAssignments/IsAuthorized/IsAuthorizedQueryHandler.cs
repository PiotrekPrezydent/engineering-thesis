using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.IsAuthorized;

// default deny: only an assignment of the sender on that resource (or a global one) whose role has the permission allows
public class IsAuthorizedQueryHandler : IQueryHandler<IsAuthorizedQuery, bool>
{
    private readonly IReadModel _readModel;

    public IsAuthorizedQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<bool> HandleAsync(IsAuthorizedQuery query)
    {
        var held = await SubjectAccess.ReadAsync(_readModel, query.SenderId, query.ScopeId);
        return held.Any(h => h.PermissionCodes.Contains(query.PermissionCode));
    }
}
