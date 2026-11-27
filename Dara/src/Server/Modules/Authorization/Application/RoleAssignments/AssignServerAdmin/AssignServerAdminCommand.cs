using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.AssignServerAdmin;

// the seed of the host makes a subject the administrator of the server (Authorization.ServerAdmin on every resource
// of Authorization.Server); no endpoint sends it
public record AssignServerAdminCommand(Guid SubjectId) : ICommand;
