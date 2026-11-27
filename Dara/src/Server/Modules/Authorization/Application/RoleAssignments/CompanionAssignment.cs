namespace Dara.Server.Modules.Authorization.Application.RoleAssignments;

// a companion role of the granted or revoked role on one resource of its own (child) scope, e.g.
// Plugins.FunctionOwner on the id of one function of the plugin; the scope follows from the role
public record CompanionAssignment(string RoleCode, Guid ScopeId);
