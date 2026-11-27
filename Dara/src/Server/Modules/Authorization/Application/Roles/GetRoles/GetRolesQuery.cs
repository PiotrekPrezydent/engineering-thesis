using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Authorization.Application.Roles.GetRoles;

// the catalog for the panel: every role of every module with the permissions it allows
public record GetRolesQuery : IQuery<List<RoleDto>>;
