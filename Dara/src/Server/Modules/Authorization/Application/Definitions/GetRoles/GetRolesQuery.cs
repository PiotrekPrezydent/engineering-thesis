using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Authorization.Application.Definitions.GetRoles;

// every role the modules declare, so a client knows what it can grant
public record GetRolesQuery : IQuery<List<RoleDto>>;
