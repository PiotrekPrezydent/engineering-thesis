using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.IsAuthorized;

// may the sender (whoever calls: a client by its token, a user by the panel) do this on that resource; the only
// question the host asks Authorization, before an endpoint or a hub method marked with the permission
public record IsAuthorizedQuery(Guid SenderId, string PermissionCode, Guid ScopeId) : IQuery<bool>;
