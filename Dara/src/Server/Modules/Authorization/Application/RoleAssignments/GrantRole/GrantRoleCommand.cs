using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.GrantRole;

// the sender (whoever calls) gives a subject (any id) a role on one resource of the scope of the role, and its
// companion roles on their own resources (e.g. Plugins.Owner on a plugin and Plugins.FunctionOwner on each function);
// all or nothing
public record GrantRoleCommand(Guid SenderId, Guid SubjectId, string RoleCode, Guid ScopeId,
    IReadOnlyList<CompanionAssignment>? Companions = null) : ICommand;
