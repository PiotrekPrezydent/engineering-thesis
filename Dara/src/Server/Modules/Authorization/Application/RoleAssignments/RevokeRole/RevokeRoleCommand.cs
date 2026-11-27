using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.RevokeRole;

// the sender takes a role on one resource from a subject, and the companion roles it names on their resources, on
// the same terms as a grant; all or nothing
public record RevokeRoleCommand(Guid SenderId, Guid SubjectId, string RoleCode, Guid ScopeId,
    IReadOnlyList<CompanionAssignment>? Companions = null) : ICommand;
